using System;
using System.Collections.Generic;

namespace LightValidation.Abstractions.Build;

public interface ICollectionRuleChainConfiguration<TEntity, TProperty>
    : IRuleChainBuilder<TEntity, IEnumerable<TProperty>>
{
    void SetIndexBuilder(Action<CollectionIndexContext<TEntity, TProperty>> indexBuilder);
}
