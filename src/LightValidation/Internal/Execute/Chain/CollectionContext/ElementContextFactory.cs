using LightValidation.Abstractions.Execute;
using System;
using System.Collections.Generic;

namespace LightValidation.Internal.Execute.Chain.CollectionContext;

internal interface IElementContextFactory
{
    IElementContext<TEntity, TProperty> Create<TEntity, TProperty>(
        Action<CollectionIndexContext<TEntity, TProperty>> indexBuilder,
        IPropertyValidationContext<TEntity, IEnumerable<TProperty>?> collectionContext,
        IList<TProperty> elements);
}

internal sealed class ElementContextFactory : IElementContextFactory
{
    public IElementContext<TEntity, TProperty> Create<TEntity, TProperty>(
        Action<CollectionIndexContext<TEntity, TProperty>> indexBuilder,
        IPropertyValidationContext<TEntity, IEnumerable<TProperty>?> collectionContext,
        IList<TProperty> elements)
    {
        return new ElementContext<TEntity, TProperty>(
            indexBuilder, collectionContext, DependencyResolver.CollectionMetadataFactory)
        {
            Elements = elements,
        };
    }
}
