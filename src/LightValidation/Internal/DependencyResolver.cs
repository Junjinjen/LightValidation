using LightValidation.Internal.Build.Chain;
using LightValidation.Internal.Build.Nested;
using LightValidation.Internal.Build.Property;
using LightValidation.Internal.Build.Rule;
using LightValidation.Internal.Build.Rule.Context;
using LightValidation.Internal.Build.Rule.DefaultRuleBuild;
using LightValidation.Internal.Build.Rule.FailureGeneration.MetadataProvision;
using LightValidation.Internal.Build.Scope;
using LightValidation.Internal.Build.Scope.Track;
using LightValidation.Internal.Build.Validation;
using LightValidation.Internal.Build.Validation.Context;
using LightValidation.Internal.Execute.Chain;
using LightValidation.Internal.Execute.Chain.CollectionContext;
using LightValidation.Internal.Execute.Chain.CollectionContext.Metadata;
using LightValidation.Internal.Execute.Nested;
using LightValidation.Internal.Execute.Nested.Context;
using LightValidation.Internal.Execute.Property;
using LightValidation.Internal.Execute.Property.Context;
using LightValidation.Internal.Execute.Rule;
using LightValidation.Internal.Execute.Rule.FailureGeneration;
using LightValidation.Internal.Execute.Rule.FailureGeneration.DescriptionGeneration;
using LightValidation.Internal.Execute.Rule.FailureGeneration.MetadataGeneration;
using LightValidation.Internal.Execute.Scope;
using LightValidation.Internal.Execute.Validation;

namespace LightValidation.Internal;

internal static class DependencyResolver
{
    public static IValidationExecutorCache ValidationExecutorCache { get; set; }
        = new ValidationExecutorCache();

    public static ICollectionRuleChainBuilderFactory CollectionRuleChainBuilderFactory { get; set; }
        = new CollectionRuleChainBuilderFactory();

    public static IRuleChainBuilderFactory RuleChainBuilderFactory { get; set; }
        = new RuleChainBuilderFactory();

    public static INestedValidationBuilderFactory NestedValidationBuilderFactory { get; set; }
        = new NestedValidationBuilderFactory();

    public static Build.Property.Context.IPropertyContextFactory PropertyBuildContextFactory { get; set; }
        = new Build.Property.Context.PropertyContextFactory();

    public static IPropertyValidationBuilderFactory PropertyValidationBuilderFactory { get; set; }
        = new PropertyValidationBuilderFactory();

    public static IRuleContextFactory RuleContextFactory { get; set; }
        = new RuleContextFactory();

    public static IDefaultRuleBuilderFactory DefaultRuleBuilderFactory { get; set; }
        = new DefaultRuleBuilderFactory();

    public static IMetadataProviderFactory MetadataProviderFactory { get; set; }
        = new MetadataProviderFactory();

    public static IRuleValidationBuilderFactory RuleValidationBuilderFactory { get; set; }
        = new RuleValidationBuilderFactory();

    public static IScopeTrackerFactory ScopeTrackerFactory { get; set; }
        = new ScopeTrackerFactory();

    public static IConditionalScopeBuilderFactory ConditionalScopeBuilderFactory { get; set; }
        = new ConditionalScopeBuilderFactory();

    public static IDependentScopeBuilderFactory DependentScopeBuilderFactory { get; set; }
        = new DependentScopeBuilderFactory();

    public static IEntityContextFactory EntityBuildContextFactory { get; set; }
        = new EntityContextFactory();

    public static IValidationBuilderFactory ValidationBuilderFactory { get; set; }
        = new ValidationBuilderFactory();

    public static ICollectionMetadataFactory CollectionMetadataFactory { get; set; }
        = new CollectionMetadataFactory();

    public static IElementContextFactory ElementContextFactory { get; set; }
        = new ElementContextFactory();

    public static ICollectionRuleChainFactory CollectionRuleChainFactory { get; set; }
        = new CollectionRuleChainFactory();

    public static IRuleChainFactory RuleChainFactory { get; set; }
        = new RuleChainFactory();

    public static INestedContextFactory NestedContextFactory { get; set; }
        = new NestedContextFactory();

    public static INestedValidationExecutorFactory NestedValidationExecutorFactory { get; set; }
        = new NestedValidationExecutorFactory();

    public static IPropertyContextFactory PropertyValidationContextFactory { get; set; }
        = new PropertyContextFactory();

    public static IPropertyContextProviderFactory PropertyContextProviderFactory { get; set; }
        = new PropertyContextProviderFactory();

    public static IPropertyValidationExecutorFactory PropertyValidationExecutorFactory { get; set; }
        = new PropertyValidationExecutorFactory();

    public static IRuntimeDescriptionGeneratorFactory RuntimeDescriptionGeneratorFactory { get; set; }
        = new RuntimeDescriptionGeneratorFactory();

    public static IStaticDescriptionGeneratorFactory StaticDescriptionGeneratorFactory { get; set; }
        = new StaticDescriptionGeneratorFactory();

    public static IRuntimeMetadataGeneratorFactory RuntimeMetadataGeneratorFactory { get; set; }
        = new RuntimeMetadataGeneratorFactory();

    public static IStaticMetadataGeneratorFactory StaticMetadataGeneratorFactory { get; set; }
        = new StaticMetadataGeneratorFactory();

    public static IRuleFailureGeneratorFactory RuleFailureGeneratorFactory { get; set; }
        = new RuleFailureGeneratorFactory();

    public static IRuleValidationExecutorFactory RuleValidationExecutorFactory { get; set; }
        = new RuleValidationExecutorFactory();

    public static IConditionalScopeFactory ConditionalScopeFactory { get; set; }
        = new ConditionalScopeFactory();

    public static IDependentScopeFactory DependentScopeFactory { get; set; }
        = new DependentScopeFactory();

    public static Execute.Validation.Context.IEntityContextFactory EntityValidationContextFactory { get; set; }
        = new Execute.Validation.Context.EntityContextFactory();

    public static IValidationExecutorFactory ValidationExecutorFactory { get; set; }
        = new ValidationExecutorFactory();

    public static IValidationOptionsFactory ValidationOptionsFactory { get; set; }
        = new ValidationOptionsFactory();
}
