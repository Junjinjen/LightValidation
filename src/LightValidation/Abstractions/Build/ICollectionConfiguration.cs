using System;
using System.Collections.Generic;

namespace LightValidation.Abstractions.Build;

public interface ICollectionConfiguration<TEntity, TProperty> : IRuleChainBuilder<TEntity, IEnumerable<TProperty>>
{
    void SetIndexBuilder(Action<CollectionIndexContext<TEntity, TProperty>> indexBuilder);
}
