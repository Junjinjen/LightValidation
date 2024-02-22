using LightValidation.Abstractions.Build;
using System;

namespace LightValidation.Extensions;

public static class CollectionConfigurationExtensions
{
    public static ICollectionConfiguration<TEntity, TProperty> WithIndexBuilder<TEntity, TProperty>(
        this ICollectionConfiguration<TEntity, TProperty> collectionConfiguration,
        Action<CollectionIndexContext<TEntity, TProperty>> indexBuilder)
    {
        ArgumentNullException.ThrowIfNull(collectionConfiguration);
        ArgumentNullException.ThrowIfNull(indexBuilder);

        collectionConfiguration.SetIndexBuilder(indexBuilder);

        return collectionConfiguration;
    }
}
