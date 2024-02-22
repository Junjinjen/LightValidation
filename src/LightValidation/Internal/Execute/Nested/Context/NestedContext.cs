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
        var propertyName = GetPropertyName(failure);

        var propertyFailure = new RuleFailure
        {
            PropertyName = propertyName,
            PropertyValue = failure.PropertyValue,
            ErrorCode = failure.ErrorCode,
            ErrorDescription = failure.ErrorDescription,
            ErrorMetadata = failure.ErrorMetadata,
        };

        _propertyContext.AddRuleFailure(propertyFailure);
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

    private string GetPropertyName(RuleFailure failure)
    {
        var nestedPropertyName = failure.PropertyName;
        var isNullEntityFailure = failure is NullEntityFailure;
        if (_propertyContext.CollectionIndexBuilder == null)
        {
            return isNullEntityFailure ? _propertyName : $"{_propertyName}.{nestedPropertyName}";
        }

        var builder = new StringBuilder(_propertyName);
        _propertyContext.CollectionIndexBuilder.Invoke(builder);

        if (!isNullEntityFailure)
        {
            builder.Append(CultureInfo.InvariantCulture, $".{nestedPropertyName}");
        }

        return builder.ToString();
    }
}
