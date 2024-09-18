using LightValidation.Abstractions.Execute;
using LightValidation.Internal.Execute.Rule.FailureGeneration.DescriptionGeneration;
using LightValidation.Internal.Execute.Rule.FailureGeneration.MetadataGeneration;
using LightValidation.Result;
using System.Text;

namespace LightValidation.Internal.Execute.Rule.FailureGeneration;

internal interface IRuleFailureGenerator<TEntity, TProperty>
{
    RuleFailure Generate(IPropertyValidationContext<TEntity, TProperty> context);
}

internal sealed class RuleFailureGenerator<TEntity, TProperty> : IRuleFailureGenerator<TEntity, TProperty>
{
    private readonly IErrorMetadataGenerator<TEntity, TProperty> _metadataGenerator;
    private readonly IErrorDescriptionGenerator<TProperty> _descriptionGenerator;

    private readonly bool _appendCollectionIndexToPropertyName;
    private readonly string _propertyName;
    private readonly string _errorCode;

    public RuleFailureGenerator(
        bool appendCollectionIndexToPropertyName,
        string propertyName,
        string errorCode,
        IErrorMetadataGenerator<TEntity, TProperty> metadataGenerator,
        IErrorDescriptionGenerator<TProperty> descriptionGenerator)
    {
        _appendCollectionIndexToPropertyName = appendCollectionIndexToPropertyName;
        _propertyName = propertyName;
        _errorCode = errorCode;

        _metadataGenerator = metadataGenerator;
        _descriptionGenerator = descriptionGenerator;
    }

    public RuleFailure Generate(IPropertyValidationContext<TEntity, TProperty> context)
    {
        var collectionIndex = GetCollectionIndex(context);

        var propertyName = GetPropertyName(collectionIndex);
        var propertyValue = context.PropertyValue;
        var metadata = _metadataGenerator.Generate(context.ValidationContext, propertyValue, collectionIndex);
        var description = _descriptionGenerator.Generate(propertyValue, metadata);

        return new RuleFailure
        {
            PropertyName = propertyName,
            PropertyValue = propertyValue,
            ErrorCode = _errorCode,
            ErrorDescription = description,
            ErrorMetadata = metadata,
        };
    }

    private static string? GetCollectionIndex(IPropertyValidationContext<TEntity, TProperty> context)
    {
        if (context.CollectionIndexBuilder == null)
        {
            return null;
        }

        var builder = new StringBuilder();
        context.CollectionIndexBuilder.Invoke(builder);

        return builder.ToString();
    }

    private string GetPropertyName(string? collectionIndex)
    {
        if (!_appendCollectionIndexToPropertyName || string.IsNullOrEmpty(collectionIndex))
        {
            return _propertyName;
        }

        return _propertyName + collectionIndex;
    }
}
