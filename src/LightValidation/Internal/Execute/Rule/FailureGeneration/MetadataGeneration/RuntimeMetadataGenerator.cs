using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LightValidation.Internal.Execute.Rule.FailureGeneration.MetadataGeneration;

internal sealed class RuntimeMetadataGenerator<TEntity, TProperty> : IErrorMetadataGenerator<TEntity, TProperty>
{
    private readonly KeyValuePair<string, Func<ValidationContext<TEntity>, TProperty, object?>>[] _metadataSelectors;
    private readonly KeyValuePair<string, object?>[]? _staticTemplate;
    private readonly bool _insertCollectionIndex;

    public RuntimeMetadataGenerator(
        Dictionary<string, Func<ValidationContext<TEntity>, TProperty, object?>> runtimeMetadata,
        Dictionary<string, object?> staticMetadata,
        bool insertCollectionIndex)
    {
        Debug.Assert(runtimeMetadata.Count != 0 || insertCollectionIndex,
            "Runtime metadata generator must have dynamic metadata or collection index.");

        _metadataSelectors = runtimeMetadata.ToArray();
        _staticTemplate = staticMetadata.Count != 0 ? staticMetadata.ToArray() : null;
        _insertCollectionIndex = insertCollectionIndex;
    }

    public IReadOnlyDictionary<string, object?> Generate(
        ValidationContext<TEntity> context, TProperty propertyValue, string? collectionIndex)
    {
        var metadata = CreateMetadata();

        for (var i = 0; i < _metadataSelectors.Length; i++)
        {
            var metadataKey = _metadataSelectors[i].Key;
            var metadataValue = _metadataSelectors[i].Value.Invoke(context, propertyValue);

            metadata.Add(metadataKey, metadataValue);
        }

        if (_staticTemplate != null)
        {
            for (var i = 0; i < _staticTemplate.Length; i++)
            {
                metadata.Add(_staticTemplate[i].Key, _staticTemplate[i].Value);
            }
        }

        if (_insertCollectionIndex)
        {
            metadata.Add(MetadataKey.CollectionIndex, collectionIndex);
        }

        return metadata;
    }

    private Dictionary<string, object?> CreateMetadata()
    {
        var capacity = _metadataSelectors.Length;
        if (_staticTemplate != null)
        {
            capacity += _staticTemplate.Length;
        }

        if (_insertCollectionIndex)
        {
            capacity++;
        }

        return new Dictionary<string, object?>(capacity);
    }
}
