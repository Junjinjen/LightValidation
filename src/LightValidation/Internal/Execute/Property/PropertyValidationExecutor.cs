using LightValidation.Abstractions.Execute;
using LightValidation.Internal.Execute.Property.Context;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LightValidation.Internal.Execute.Property;

internal sealed class PropertyValidationExecutor<TEntity, TProperty> : IEntityValidator<TEntity>
{
    private readonly IPropertyContextProvider<TEntity, TProperty> _propertyContextProvider;
    private readonly IPropertyValidator<TEntity, TProperty> _propertyValidator;

    public PropertyValidationExecutor(
        IPropertyContextProvider<TEntity, TProperty> propertyContextProvider,
        IPropertyValidator<TEntity, TProperty> propertyValidator)
    {
        _propertyContextProvider = propertyContextProvider;
        _propertyValidator = propertyValidator;
    }

    public ExecutionModeCollection ExecutionModes => _propertyValidator.ExecutionModes;

    public ValueTask Validate(IEntityValidationContext<TEntity> context, ExecutionMode currentMode)
    {
        Debug.Assert(ExecutionModes.Contains(currentMode),
            "The current execution mode is not supported by the validator.");

        var propertyContext = _propertyContextProvider.Get(context);
        if (!CanExecute(propertyContext, currentMode))
        {
            return ValueTask.CompletedTask;
        }

        return _propertyValidator.Validate(propertyContext, currentMode);
    }

    private static bool CanExecute(IPropertyValidationContext<TEntity, TProperty> context, ExecutionMode currentMode)
    {
        return currentMode != ExecutionMode.OnValidProperty || context.IsPropertyValid;
    }
}
