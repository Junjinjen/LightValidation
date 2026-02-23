using LightValidation.Internal.Selector;
using System;
using System.Linq.Expressions;

namespace LightValidation;

public interface IValidator<out TValue, TError>
{
    IValidationContext<TError> Context { get; }

    string PropertyPath { get; }

    TValue Value { get; }

    bool IsValid { get; }

    IValidator<TValue, TError> AddAlias(string alias);

    IValidator<TValue, TError> AddError(TError error);

    IValidator<TProperty, TError> Property<TProperty>(LambdaExpression selectorExpression);
}

public sealed class Validator<TValue, TError> : IValidator<TValue, TError>
{
    private readonly Func<TValue>? _valueSelector;

    private bool _isValueSelected;
    private TValue? _value;

    public Validator(TValue value)
    {
        _isValueSelected = true;
        _value = value;
    }

    private Validator(Func<TValue> valueSelector)
    {
        _valueSelector = valueSelector;
    }

    public required IValidationContext<TError> Context { get; init; }

    public required string PropertyPath { get; init; }

    public TValue Value
    {
        get
        {
            if (!_isValueSelected)
            {
                _value = _valueSelector!.Invoke();
                _isValueSelected = true;
            }

            return _value!;
        }
    }

    public bool IsValid => Context.IsPropertyValid(PropertyPath);

    public IValidator<TValue, TError> AddAlias(string alias)
    {
        Context.AddPropertyAlias(PropertyPath, alias);

        return this;
    }

    public IValidator<TValue, TError> AddError(TError error)
    {
        Context.AddPropertyError(PropertyPath, error);

        return this;
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

        var selectorInfo = SelectorParser.Parse(selectorExpression, PropertyPath);

        return new Validator<TProperty, TError>(() => SelectorInvoker.GetPropertyValue<TValue, TProperty>(Value, selectorInfo))
        {
            Context = Context,
            PropertyPath = selectorInfo.PropertyPath,
        };
    }
}
