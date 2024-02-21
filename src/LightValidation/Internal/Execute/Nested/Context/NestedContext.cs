using LightValidation.Abstractions.Execute;
using LightValidation.Internal.Execute.Validation;
using LightValidation.Result;
using System;
using System.Buffers;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace LightValidation.Internal.Execute.Nested.Context;

internal interface INestedContext<TProperty> : IEntityValidationContext<TProperty>
{
    IValidationExecutor<TProperty> ValidationExecutor { get; }
}

internal sealed class NestedContext<TEntity, TProperty> : INestedContext<TProperty>
{
    private readonly IPropertyValidationContext<TEntity, TProperty> _propertyContext;

    private readonly string _propertyName;
    private readonly object?[] _metadata;
    private readonly int _metadataCount;

    public NestedContext(
        int metadataCount, string propertyName, IPropertyValidationContext<TEntity, TProperty> propertyContext)
    {
        _metadata = ArrayPool<object?>.Shared.Rent(metadataCount);

        Debug.Assert(!Array.Exists(_metadata, x => x != null), "Rented metadata contains non-null objects.");

        _propertyName = propertyName;
        _metadataCount = metadataCount;

        _propertyContext = propertyContext;
    }

    public required ValidationContext<TProperty> ValidationContext { get; init; }

    public bool IsEntityValid => _propertyContext.IsPropertyValid;

    public required IValidationExecutor<TProperty> ValidationExecutor { get; init; }

    public object? GetValidationMetadata(int metadataId)
    {
        Debug.Assert(metadataId < _metadataCount, "Index was outside the bounds of the metadata array.");

        return _metadata[metadataId];
    }

    public void SetValidationMetadata(int metadataId, object? value)
    {
        Debug.Assert(metadataId < _metadataCount, "Index was outside the bounds of the metadata array.");

        _metadata[metadataId] = value;
    }

    public void AddRuleFailure(RuleFailure failure)
    {
        var propertyFailure = new RuleFailure
        {
            PropertyName = _propertyName,
            PropertyValue = failure.PropertyValue,
            ErrorCode = failure.ErrorCode,
            ErrorDescription = failure.ErrorDescription,
            ErrorMetadata = failure.ErrorMetadata,
        };

        var propertyNameModifier = GetPropertyNameModifier(failure);
        _propertyContext.AddRuleFailure(propertyFailure, propertyNameModifier);
    }

    public void Dispose()
    {
        foreach (var value in _metadata)
        {
            if (value is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        Array.Clear(_metadata, 0, _metadataCount);
        ArrayPool<object?>.Shared.Return(_metadata);
    }

    private static Action<StringBuilder>? GetPropertyNameModifier(RuleFailure failure)
    {
        if (failure is NullEntityFailure)
        {
            return null;
        }

        var originalPropertyName = failure.PropertyName;

        return propertyName => propertyName.Append(CultureInfo.InvariantCulture, $".{originalPropertyName}");
    }
}
