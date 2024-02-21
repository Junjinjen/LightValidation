using LightValidation.Internal.Execute.Rule.FailureGeneration.DescriptionGeneration;
using LightValidation.Internal.Execute.Rule.FailureGeneration.MetadataGeneration;
using LightValidation.Result;

namespace LightValidation.Internal.Execute.Rule.FailureGeneration;

internal interface IRuleFailureGenerator<TEntity, TProperty>
{
    RuleFailure Generate(ValidationContext<TEntity> context, TProperty propertyValue);
}

internal sealed class RuleFailureGenerator<TEntity, TProperty> : IRuleFailureGenerator<TEntity, TProperty>
{
    private readonly IErrorMetadataGenerator<TEntity, TProperty> _metadataGenerator;
    private readonly IErrorDescriptionGenerator<TProperty> _descriptionGenerator;

    private readonly string _propertyName;
    private readonly string _errorCode;

    public RuleFailureGenerator(
        string propertyName,
        string errorCode,
        IErrorMetadataGenerator<TEntity, TProperty> metadataGenerator,
        IErrorDescriptionGenerator<TProperty> descriptionGenerator)
    {
        _propertyName = propertyName;
        _errorCode = errorCode;
        _metadataGenerator = metadataGenerator;
        _descriptionGenerator = descriptionGenerator;
    }

    public RuleFailure Generate(ValidationContext<TEntity> context, TProperty propertyValue)
    {
        var metadata = _metadataGenerator.Generate(context, propertyValue);
        var description = _descriptionGenerator.Generate(propertyValue, metadata);

        return new RuleFailure
        {
            PropertyName = _propertyName,
            PropertyValue = propertyValue,
            ErrorCode = _errorCode,
            ErrorDescription = description,
            ErrorMetadata = metadata,
        };
    }
}
