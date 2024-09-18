using LightValidation.Abstractions.Execute;
using LightValidation.Internal.Execute.Chain.CollectionContext.Metadata;
using LightValidation.Result;
using System;
using System.Collections.Generic;
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

    private readonly Action<CollectionIndexContext<TEntity, TProperty>> _indexBuilder;

    public ElementContext(
        Action<CollectionIndexContext<TEntity, TProperty>> indexBuilder,
        IPropertyValidationContext<TEntity, IEnumerable<TProperty>?> collectionContext,
        ICollectionMetadataFactory collectionMetadataFactory)
    {
        _indexBuilder = indexBuilder;

        _collectionContext = collectionContext;
        _collectionMetadataFactory = collectionMetadataFactory;
    }

    public IEntityValidationContext<TEntity> EntityContext => _collectionContext.EntityContext;

    public ValidationContext<TEntity> ValidationContext => _collectionContext.ValidationContext;

    public required IList<TProperty> Elements { get; init; }

    public int ElementIndex { get; set; }

    public TProperty PropertyValue => Elements[ElementIndex];

    public bool IsPropertyValid => _collectionContext.IsPropertyValid;

    public bool IsEntityValid => _collectionContext.IsEntityValid;

    public bool CanExecuteDependentRules =>
        _collectionContext.CanExecuteDependentRules && ElementIndex == Elements.Count - 1;

    public Action<StringBuilder>? CollectionIndexBuilder => BuildCollectionIndex;

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

    public void AddRuleFailure(RuleFailure failure)
    {
        _collectionContext.AddRuleFailure(failure);
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

    private void BuildCollectionIndex(StringBuilder builder)
    {
        _collectionContext.CollectionIndexBuilder?.Invoke(builder);

        var context = new CollectionIndexContext<TEntity, TProperty>
        {
            StringBuilder = builder,
            Entity = EntityContext.ValidationContext.Entity,
            Element = PropertyValue,
            Index = ElementIndex,
        };

        _indexBuilder.Invoke(context);
    }
}
