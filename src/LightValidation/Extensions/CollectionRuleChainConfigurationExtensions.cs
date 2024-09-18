using LightValidation.Abstractions.Build;
using System;

namespace LightValidation.Extensions;

public static class CollectionRuleChainConfigurationExtensions
{
    public static ICollectionRuleChainConfiguration<TEntity, TProperty> WithIndexBuilder<TEntity, TProperty>(
        this ICollectionRuleChainConfiguration<TEntity, TProperty> configuration,
        Action<CollectionIndexContext<TEntity, TProperty>> indexBuilder)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(indexBuilder);

        configuration.SetIndexBuilder(indexBuilder);

        return configuration;
    }
}
