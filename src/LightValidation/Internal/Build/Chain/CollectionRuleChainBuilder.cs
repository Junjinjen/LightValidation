using LightValidation.Abstractions.Build;
using LightValidation.Abstractions.Execute;
using LightValidation.Internal.Build.Property;
using LightValidation.Internal.Build.Property.Context;
using LightValidation.Internal.Execute.Chain;
using System.Collections.Generic;

namespace LightValidation.Internal.Build.Chain;

internal interface ICollectionRuleChainBuilder<TEntity, TProperty>
    : IRuleChainBuilderInitial<TEntity, TProperty>, IPropertyValidatorBuilder<TEntity, IEnumerable<TProperty>?>
{
}

internal sealed class CollectionRuleChainBuilder<TEntity, TProperty>
    : RuleChainBuilderBase<TEntity, TProperty>, ICollectionRuleChainBuilder<TEntity, TProperty>
{
    private readonly ICollectionRuleChainFactory _collectionRuleChainFactory;

    public CollectionRuleChainBuilder(
        IValidationBuilder<TEntity> validationBuilder,
        IPropertyConditionBuilder<TEntity, TProperty> propertyConditionBuilder,
        IPropertyContextEditor propertyContextEditor,
        ICollectionRuleChainFactory collectionRuleChainFactory)
        : base(validationBuilder, propertyConditionBuilder, propertyContextEditor)
    {
        _collectionRuleChainFactory = collectionRuleChainFactory;
    }

    public IPropertyValidator<TEntity, IEnumerable<TProperty>?>? Build(IPropertyBuildContext context)
    {
        SetBuilt();

        var propertyValidators = BuildPropertyValidators(context);
        if (propertyValidators.Length == 0)
        {
            return null;
        }

        var condition = BuildCondition();
        var metadataId = context.RegisterMetadata();

        return _collectionRuleChainFactory.Create(condition, propertyValidators, metadataId);
    }
}
