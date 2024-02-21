using System.Collections.Generic;

namespace LightValidation.Internal.Execute.Rule.FailureGeneration.DescriptionGeneration;

internal interface IErrorDescriptionGenerator<TProperty>
{
    string Generate(TProperty propertyValue, IReadOnlyDictionary<string, object?> errorMetadata);
}
