using System.Collections.Generic;

namespace LightValidation.Internal.Execute.Rule.FailureGeneration.MetadataGeneration;

internal interface IStaticMetadataGeneratorFactory
{
    IErrorMetadataGenerator<TEntity, TProperty> Create<TEntity, TProperty>(Dictionary<string, object?> staticMetadata);
}

internal sealed class StaticMetadataGeneratorFactory : IStaticMetadataGeneratorFactory
{
    public IErrorMetadataGenerator<TEntity, TProperty> Create<TEntity, TProperty>(
        Dictionary<string, object?> staticMetadata)
    {
        return new StaticMetadataGenerator<TEntity, TProperty>(staticMetadata);
    }
}
