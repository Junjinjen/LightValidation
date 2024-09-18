using LightValidation.Abstractions.Execute;
using LightValidation.Internal.Execute.Nested.Context;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LightValidation.Internal.Execute.Nested;

internal sealed class NestedValidationExecutor<TEntity, TProperty> : IPropertyValidator<TEntity, TProperty>
{
    private readonly IValidationExecutorCache _validationExecutorCache;
    private readonly INestedContextFactory _nestedContextFactory;

    private readonly Func<ValidationContext<TEntity>, IValidatorInternal<TProperty>> _validatorProvider;
    private readonly Func<ValidationContext<TEntity>, TProperty, ValueTask<bool>>? _condition;
    private readonly string _propertyName;
    private readonly int _metadataId;

    public NestedValidationExecutor(
        Func<ValidationContext<TEntity>, IValidatorInternal<TProperty>> validatorProvider,
        Func<ValidationContext<TEntity>, TProperty, ValueTask<bool>>? condition,
        string propertyName,
        int metadataId,
        IValidationExecutorCache validationExecutorCache,
        INestedContextFactory nestedContextFactory)
    {
        ExecutionModes = new([ExecutionMode.Always, ExecutionMode.OnValidEntity]);

        _validatorProvider = validatorProvider;
        _condition = condition;
        _propertyName = propertyName;
        _metadataId = metadataId;

        _validationExecutorCache = validationExecutorCache;
        _nestedContextFactory = nestedContextFactory;
    }

    public ExecutionModeCollection ExecutionModes { get; }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
    public async ValueTask Validate(IPropertyValidationContext<TEntity, TProperty> context, ExecutionMode currentMode)
    {
        Debug.Assert(ExecutionModes.Contains(currentMode),
            "The current execution mode is not supported by the validator.");

        var nestedContext = await GetNestedContext(context).ConfigureAwait(false);
        if (nestedContext == null)
        {
            return;
        }

        if (currentMode == ExecutionMode.OnValidEntity)
        {
            await ExecuteValidator(nestedContext, ExecutionMode.OnValidEntity).ConfigureAwait(false);

            return;
        }

        await ExecuteValidator(nestedContext, ExecutionMode.Always).ConfigureAwait(false);
        await ExecuteValidator(nestedContext, ExecutionMode.OnValidProperty).ConfigureAwait(false);
    }

    private static ValueTask ExecuteValidator(INestedContext<TProperty> context, ExecutionMode mode)
    {
        var validationExecutor = context.ValidationExecutor;
        if (!validationExecutor.ExecutionModes.Contains(mode))
        {
            return ValueTask.CompletedTask;
        }

        return validationExecutor.Validate(context, mode);
    }

    private ValueTask<INestedContext<TProperty>?> GetNestedContext(
        IPropertyValidationContext<TEntity, TProperty> context)
    {
        var value = context.GetValidationMetadata(_metadataId);
        if (value != null)
        {
            var result = ReferenceEquals(value, Constants.FalseObjectValue) ? null : value;

            return ValueTask.FromResult((INestedContext<TProperty>?)result);
        }

        if (_condition == null)
        {
            var result = CreateNestedContext(context);

            return ValueTask.FromResult(result);
        }

        return CreateNestedContextWithConditionCheck(context);
    }

    private INestedContext<TProperty>? CreateNestedContext(IPropertyValidationContext<TEntity, TProperty> context)
    {
        var validator = _validatorProvider.Invoke(context.ValidationContext);
        validator.SetDependencyResolver(context.ValidationContext);

        var validationExecutor = _validationExecutorCache.Get(validator);

        var validationContextParameters = new ValidationContextParameters<TProperty>
        {
            Entity = context.PropertyValue,
            Cache = context.ValidationContext.Cache,
            RuleSets = context.ValidationContext.RuleSets,
            CancellationToken = context.ValidationContext.CancellationToken,
        };

        var validationContext = validator.CreateValidationContext(validationContextParameters);

        var nestedContextParameters = new NestedContextParameters<TEntity, TProperty>
        {
            MetadataCount = validationExecutor.MetadataCount,
            PropertyName = _propertyName,
            PropertyContext = context,
            ValidationContext = validationContext,
            ValidationExecutor = validationExecutor,
        };

        var nestedContext = _nestedContextFactory.Create(nestedContextParameters);

        context.SetValidationMetadata(_metadataId, nestedContext);

        return nestedContext;
    }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
    private async ValueTask<INestedContext<TProperty>?> CreateNestedContextWithConditionCheck(
        IPropertyValidationContext<TEntity, TProperty> context)
    {
        var condition = await _condition!
            .Invoke(context.ValidationContext, context.PropertyValue).ConfigureAwait(false);

        if (!condition)
        {
            context.SetValidationMetadata(_metadataId, Constants.FalseObjectValue);

            return null;
        }

        return CreateNestedContext(context);
    }
}
