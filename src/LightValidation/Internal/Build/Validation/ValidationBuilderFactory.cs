using LightValidation.Internal.Build.Property;
using LightValidation.Internal.Build.Property.Context;

namespace LightValidation.Internal.Build.Validation;

internal interface IValidationBuilderFactory
{
    IValidationBuilderInternal<TEntity> Create<TEntity>();
}

internal sealed class ValidationBuilderFactory : IValidationBuilderFactory
{
    public IValidationBuilderInternal<TEntity> Create<TEntity>()
    {
        var propertySelectorCache = new PropertySelectorCache<TEntity>();
        var propertyContextProviderCache = new PropertyContextProviderCache<TEntity>(
            DependencyResolver.PropertyContextProviderFactory, propertySelectorCache);
        var propertyContextCache = new PropertyContextCache<TEntity>(
            propertySelectorCache, DependencyResolver.PropertyBuildContextFactory);
        var nullEntityFailureBuilder = new NullEntityFailureBuilder();

        return new ValidationBuilder<TEntity>(
            DependencyResolver.ConditionalScopeBuilderFactory,
            propertyContextProviderCache,
            DependencyResolver.PropertyValidationBuilderFactory,
            DependencyResolver.ValidationExecutorFactory,
            propertyContextCache,
            nullEntityFailureBuilder,
            DependencyResolver.EntityBuildContextFactory,
            DependencyResolver.ScopeTrackerFactory);
    }
}
