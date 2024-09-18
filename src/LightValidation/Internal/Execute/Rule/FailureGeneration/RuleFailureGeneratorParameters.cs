using LightValidation.Internal.Execute.Rule.FailureGeneration.DescriptionGeneration;
using LightValidation.Internal.Execute.Rule.FailureGeneration.MetadataGeneration;

namespace LightValidation.Internal.Execute.Rule.FailureGeneration;

internal readonly ref struct RuleFailureGeneratorParameters<TEntity, TProperty>
{
    public required bool AppendCollectionIndexToPropertyName { get; init; }

    public required string PropertyName { get; init; }

    public required string ErrorCode { get; init; }

    public required IErrorMetadataGenerator<TEntity, TProperty> MetadataGenerator { get; init; }

    public required IErrorDescriptionGenerator<TProperty> DescriptionGenerator { get; init; }
}
