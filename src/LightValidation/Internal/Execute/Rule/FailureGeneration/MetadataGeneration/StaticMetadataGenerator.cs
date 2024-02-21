using System.Collections.Frozen;
using System.Collections.Generic;

namespace LightValidation.Internal.Execute.Rule.FailureGeneration.MetadataGeneration;

internal sealed class StaticMetadataGenerator<TEntity, TProperty> : IErrorMetadataGenerator<TEntity, TProperty>
{
    private readonly IReadOnlyDictionary<string, object?> _metadata;

    public StaticMetadataGenerator(Dictionary<string, object?> staticMetadata)
    {
        _metadata = staticMetadata.Count != 0 ? staticMetadata : FrozenDictionary<string, object?>.Empty;
    }

    public IReadOnlyDictionary<string, object?> Generate(ValidationContext<TEntity> context, TProperty propertyValue)
    {
        return _metadata;
    }
}
