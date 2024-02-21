using System.Collections.Generic;

namespace LightValidation.Internal.Execute.Rule.FailureGeneration.DescriptionGeneration;

internal sealed class StaticDescriptionGenerator<TProperty> : IErrorDescriptionGenerator<TProperty>
{
    private readonly string _description;

    public StaticDescriptionGenerator(
        string propertyName, string description, Dictionary<string, object?> staticMetadata)
    {
        _description = description.Format(MetadataKey.PropertyName, propertyName);
        foreach (var metadata in staticMetadata)
        {
            _description = _description.Format(metadata.Key, metadata.Value);
        }
    }

    public string Generate(TProperty propertyValue, IReadOnlyDictionary<string, object?> errorMetadata)
    {
        return _description;
    }
}
