using System.Collections.Generic;

namespace LightValidation.Internal.Execute.Rule.FailureGeneration.MetadataGeneration;

internal interface IErrorMetadataGenerator<TEntity, TProperty>
{
    IReadOnlyDictionary<string, object?> Generate(ValidationContext<TEntity> context, TProperty propertyValue);
}
