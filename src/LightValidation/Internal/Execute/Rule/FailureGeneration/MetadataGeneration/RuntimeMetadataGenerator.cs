using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LightValidation.Internal.Execute.Rule.FailureGeneration.MetadataGeneration;

internal sealed class RuntimeMetadataGenerator<TEntity, TProperty> : IErrorMetadataGenerator<TEntity, TProperty>
{
    private readonly KeyValuePair<string, Func<ValidationContext<TEntity>, TProperty, object?>>[] _metadataSelectors;
    private readonly KeyValuePair<string, object?>[]? _staticTemplate;

    public RuntimeMetadataGenerator(
        Dictionary<string, Func<ValidationContext<TEntity>, TProperty, object?>> runtimeMetadata,
        Dictionary<string, object?> staticMetadata)
    {
        Debug.Assert(runtimeMetadata.Count != 0, "Runtime metadata generator must have dynamic metadata.");

        _metadataSelectors = runtimeMetadata.ToArray();
        _staticTemplate = staticMetadata.Count != 0 ? staticMetadata.ToArray() : null;
    }

    public IReadOnlyDictionary<string, object?> Generate(ValidationContext<TEntity> context, TProperty propertyValue)
    {
        var capacity = _metadataSelectors.Length + _staticTemplate?.Length ?? 0;
        var metadata = new Dictionary<string, object?>(capacity);

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

        return metadata;
    }
}
