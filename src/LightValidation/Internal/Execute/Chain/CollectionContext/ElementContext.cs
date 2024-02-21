using LightValidation.Abstractions.Execute;
using LightValidation.Internal.Execute.Chain.CollectionContext.Metadata;
using LightValidation.Result;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace LightValidation.Internal.Execute.Chain.CollectionContext;

internal interface IElementContext<TEntity, TProperty> : IPropertyValidationContext<TEntity, TProperty>
{
    IList<TProperty> Elements { get; }

    int ElementIndex { get; set; }
}

internal sealed class ElementContext<TEntity, TProperty> : IElementContext<TEntity, TProperty>
{
    private readonly IPropertyValidationContext<TEntity, IEnumerable<TProperty>?> _collectionContext;
    private readonly ICollectionMetadataFactory _collectionMetadataFactory;

    public ElementContext(
        IPropertyValidationContext<TEntity, IEnumerable<TProperty>?> collectionContext,
        ICollectionMetadataFactory collectionMetadataFactory)
    {
        _collectionContext = collectionContext;
        _collectionMetadataFactory = collectionMetadataFactory;
    }

    public IEntityValidationContext<TEntity> EntityValidationContext => _collectionContext.EntityValidationContext;

    public ValidationContext<TEntity> ValidationContext => _collectionContext.ValidationContext;

    public required IList<TProperty> Elements { get; init; }

    public int ElementIndex { get; set; }

    public TProperty PropertyValue => Elements[ElementIndex];

    public bool IsPropertyValid => _collectionContext.IsPropertyValid;

    public bool IsEntityValid => _collectionContext.IsEntityValid;

    public bool CanExecuteDependentRules =>
        _collectionContext.CanExecuteDependentRules && ElementIndex == Elements.Count - 1;

    public object? GetValidationMetadata(int metadataId)
    {
        var metadata = GetCollectionMetadata(metadataId);

        return metadata.GetElementMetadata(ElementIndex);
    }

    public void SetValidationMetadata(int metadataId, object? value)
    {
        var metadata = GetCollectionMetadata(metadataId);
        metadata.SetElementMetadata(ElementIndex, value);
    }

    public void AddRuleFailure(RuleFailure failure, Action<StringBuilder>? propertyNameModifier = null)
    {
        var index = ElementIndex;
        _collectionContext.AddRuleFailure(failure, propertyName =>
        {
            propertyName.Append(CultureInfo.InvariantCulture, $"[{index}]");

            propertyNameModifier?.Invoke(propertyName);
        });
    }

    private ICollectionMetadata GetCollectionMetadata(int metadataId)
    {
        var value = _collectionContext.GetValidationMetadata(metadataId);
        if (value == null)
        {
            value = _collectionMetadataFactory.Create(Elements.Count);

            _collectionContext.SetValidationMetadata(metadataId, value);
        }

        return (ICollectionMetadata)value;
    }
}
