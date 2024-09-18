using LightValidation.Abstractions.Build;
using LightValidation.Abstractions.Execute;
using LightValidation.Internal.Build.Property;
using LightValidation.Internal.Build.Property.Context;
using LightValidation.Internal.Execute.Chain;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace LightValidation.Internal.Build.Chain;

internal interface ICollectionRuleChainBuilder<TEntity, TProperty>
    : ICollectionRuleChainConfiguration<TEntity, TProperty>,
    IRuleChainConfiguration<TEntity, TProperty>,
    IPropertyValidatorBuilder<TEntity, IEnumerable<TProperty>?>
{
}

internal sealed class CollectionRuleChainBuilder<TEntity, TProperty>
    : RuleChainBuilderBase<TEntity, TProperty>, ICollectionRuleChainBuilder<TEntity, TProperty>
{
    private static readonly Action<CollectionIndexContext<TEntity, TProperty>> DefaultIndexBuilder
        = context => context.StringBuilder.Append(CultureInfo.InvariantCulture, $"[{context.Index}]");

    private readonly IRuleChainBuilder<TEntity, IEnumerable<TProperty>> _ruleChainBuilder;
    private readonly ICollectionRuleChainFactory _collectionRuleChainFactory;

    private Action<CollectionIndexContext<TEntity, TProperty>> _indexBuilder = DefaultIndexBuilder;

    public CollectionRuleChainBuilder(
        IPropertyConditionBuilder<TEntity, TProperty> propertyConditionBuilder,
        IPropertyContextEditor propertyContextEditor,
        IRuleChainBuilder<TEntity, IEnumerable<TProperty>> ruleChainBuilder,
        ICollectionRuleChainFactory collectionRuleChainFactory)
        : base(ruleChainBuilder.ValidationBuilder, propertyConditionBuilder, propertyContextEditor)
    {
        _ruleChainBuilder = ruleChainBuilder;
        _collectionRuleChainFactory = collectionRuleChainFactory;
    }

    public void SetIndexBuilder(Action<CollectionIndexContext<TEntity, TProperty>> indexBuilder)
    {
        ArgumentNullException.ThrowIfNull(indexBuilder);
        EnsureNotBuilt();

        _indexBuilder = indexBuilder;
    }

    public void AddPropertyValidator(IPropertyValidatorBuilder<TEntity, IEnumerable<TProperty>> validatorBuilder)
    {
        EnsureNotBuilt();

        _ruleChainBuilder.AddPropertyValidator(validatorBuilder);
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
        var metadataId = context.RegisterValidationMetadata();

        var parameters = new CollectionRuleChainParameters<TEntity, TProperty>
        {
            Condition = condition,
            PropertyValidators = propertyValidators,
            MetadataId = metadataId,
            IndexBuilder = _indexBuilder,
        };

        return _collectionRuleChainFactory.Create(parameters);
    }
}
