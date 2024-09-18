using LightValidation.Abstractions.Build;
using LightValidation.Abstractions.Execute;
using LightValidation.Internal.Build.Property;
using LightValidation.Internal.Build.Property.Context;
using LightValidation.Internal.Execute.Chain;
using System.Linq;

namespace LightValidation.Internal.Build.Chain;

internal interface IRuleChainBuilderInternal<TEntity, TProperty>
    : IRuleChainConfiguration<TEntity, TProperty>, IPropertyValidatorBuilder<TEntity, TProperty>
{
}

internal sealed class RuleChainBuilder<TEntity, TProperty>
    : RuleChainBuilderBase<TEntity, TProperty>, IRuleChainBuilderInternal<TEntity, TProperty>
{
    private readonly IRuleChainFactory _ruleChainFactory;

    public RuleChainBuilder(
        IValidationBuilder<TEntity> validationBuilder,
        IPropertyConditionBuilder<TEntity, TProperty> propertyConditionBuilder,
        IPropertyContextEditor propertyContextEditor,
        IRuleChainFactory ruleChainFactory)
        : base(validationBuilder, propertyConditionBuilder, propertyContextEditor)
    {
        _ruleChainFactory = ruleChainFactory;
    }

    public IPropertyValidator<TEntity, TProperty>? Build(IPropertyBuildContext context)
    {
        SetBuilt();

        var propertyValidators = BuildPropertyValidators(context);
        if (propertyValidators.Length == 0)
        {
            return null;
        }

        var condition = BuildCondition();
        if (propertyValidators.Length == 1 && condition == null)
        {
            return propertyValidators.Single();
        }

        var metadataId = condition != null ? context.RegisterValidationMetadata() : Constants.InvalidMetadataId;

        return _ruleChainFactory.Create(condition, propertyValidators, metadataId);
    }
}
