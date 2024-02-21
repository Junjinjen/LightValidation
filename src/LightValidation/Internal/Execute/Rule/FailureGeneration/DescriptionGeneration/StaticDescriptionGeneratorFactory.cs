using System.Collections.Generic;

namespace LightValidation.Internal.Execute.Rule.FailureGeneration.DescriptionGeneration;

internal interface IStaticDescriptionGeneratorFactory
{
    IErrorDescriptionGenerator<TProperty> Create<TProperty>(
        string propertyName, string description, Dictionary<string, object?> staticMetadata);
}

internal sealed class StaticDescriptionGeneratorFactory : IStaticDescriptionGeneratorFactory
{
    public IErrorDescriptionGenerator<TProperty> Create<TProperty>(
        string propertyName, string description, Dictionary<string, object?> staticMetadata)
    {
        return new StaticDescriptionGenerator<TProperty>(propertyName, description, staticMetadata);
    }
}
