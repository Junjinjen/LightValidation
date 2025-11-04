using LightValidation.Internal;
using System;
using System.Linq.Expressions;

namespace LightValidation;

public interface IValidator<TError>
{
    IValidationContext<TError> Context { get; }

    LambdaExpression PathExpression { get; }

    bool IsRootValid { get; }

    bool IsValid { get; }

    object? Value { get; }

    IValidator<TError> AddError(TError error);
}

public interface IValidator<out TValue, TError> : IValidator<TError>
{
    new TValue Value { get; }

    new IValidator<TValue, TError> AddError(TError error);

    IValidator<TProperty, TError> Property<TProperty>(LambdaExpression selectorExpression);
}

internal sealed partial class Validator<TValue, TError> : IValidator<TValue, TError>
{
    private readonly PropertyNode<TError> _propertyNode;

    public Validator(PropertyNode<TError> propertyNode)
    {
        _propertyNode = propertyNode;
    }

    public required IValidationContext<TError> Context { get; init; }

    public required LambdaExpression PathExpression { get; init; }

    public bool IsRootValid => _propertyNode.IsRootValid;

    public bool IsValid => _propertyNode.IsValid;

    public required TValue Value { get; init; }

    object? IValidator<TError>.Value => Value;

    public IValidator<TValue, TError> AddError(TError error)
    {
        _propertyNode.SetInvalid();
        Context.AddError(error);

        return this;
    }

    IValidator<TError> IValidator<TError>.AddError(TError error)
    {
        return AddError(error);
    }

    public IValidator<TProperty, TError> Property<TProperty>(LambdaExpression selectorExpression)
    {
        ArgumentNullException.ThrowIfNull(selectorExpression);
        if (selectorExpression.Parameters.Count != 1
            || !selectorExpression.Parameters[0].Type.IsAssignableFrom(typeof(TValue))
            || !selectorExpression.ReturnType.IsAssignableTo(typeof(TProperty)))
        {
            throw new ArgumentException("Invalid selector expression type.", nameof(selectorExpression));
        }

        var selectorInfo = SelectorParser.Parse(selectorExpression);
        return _propertyNode.GetValidator(selectorInfo, propertyNode =>
        {
            var selector = SelectorCache.GetPropertySelector<TValue, TProperty>(selectorInfo);
            var pathExpression = PathExpressionBuilder.Build(PathExpression, selectorExpression, selectorInfo.Constants);

            ArgumentNullException.ThrowIfNull(Value);
            var value = selector.Invoke(Value);

            return new Validator<TProperty, TError>(propertyNode)
            {
                Context = Context,
                PathExpression = pathExpression,
                Value = value,
            };
        });
    }
}
