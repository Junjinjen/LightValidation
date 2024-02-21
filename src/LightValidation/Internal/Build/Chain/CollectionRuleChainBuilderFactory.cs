using LightValidation.Abstractions.Build;
using LightValidation.Internal.Build.Property;
using LightValidation.Internal.Build.Property.Context;

namespace LightValidation.Internal.Build.Chain;

internal interface ICollectionRuleChainBuilderFactory
{
    ICollectionRuleChainBuilder<TEntity, TProperty> Create<TEntity, TProperty>(
        IValidationBuilder<TEntity> validationBuilder);
}

internal sealed class CollectionRuleChainBuilderFactory : ICollectionRuleChainBuilderFactory
{
    public ICollectionRuleChainBuilder<TEntity, TProperty> Create<TEntity, TProperty>(
        IValidationBuilder<TEntity> validationBuilder)
    {
        var propertyConditionBuilder = new PropertyConditionBuilder<TEntity, TProperty>();
        var propertyContextEditor = new PropertyContextEditor(DependencyResolver.PropertyBuildContextFactory);

        return new CollectionRuleChainBuilder<TEntity, TProperty>(
            validationBuilder,
            propertyConditionBuilder,
            propertyContextEditor,
            DependencyResolver.CollectionRuleChainFactory);
    }
}
