using LightValidation.Abstractions.Execute;
using LightValidation.Result;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;

namespace LightValidation.Internal.Execute.Validation.Context;

internal sealed class EntityContext<TEntity> : IEntityValidationContext<TEntity>
{
    private readonly List<RuleFailure> _brokenRules;
    private readonly object?[] _metadata;
    private readonly int _metadataCount;

    public EntityContext(List<RuleFailure> brokenRules, int metadataCount)
    {
        _metadata = ArrayPool<object?>.Shared.Rent(metadataCount);

        Debug.Assert(!Array.Exists(_metadata, x => x != null), "Rented metadata contains non-null objects.");

        _brokenRules = brokenRules;
        _metadataCount = metadataCount;
    }

    public required ValidationContext<TEntity> ValidationContext { get; init; }

    public bool IsEntityValid => _brokenRules.Count == 0;

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
        _brokenRules.Add(failure);
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
}
