﻿using LightValidation.Abstractions.Execute;
using LightValidation.Internal.Execute.Chain.CollectionContext;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LightValidation.Internal.Execute.Chain;

internal sealed class CollectionRuleChain<TEntity, TProperty>
    : RuleChainBase<TEntity, TProperty, IEnumerable<TProperty>?>
{
    private readonly IElementContextFactory _elementContextFactory;

    public CollectionRuleChain(
        Func<ValidationContext<TEntity>, TProperty, ValueTask<bool>>? condition,
        IPropertyValidator<TEntity, TProperty>[] propertyValidators,
        int metadataId,
        IElementContextFactory elementContextFactory)
        : base(condition, propertyValidators, metadataId)
    {
        _elementContextFactory = elementContextFactory;
    }

    public override async ValueTask Validate(
        IPropertyValidationContext<TEntity, IEnumerable<TProperty>?> context, ExecutionMode currentMode)
    {
        Debug.Assert(ExecutionModes.Contains(currentMode),
            "The current execution mode is not supported by the validator.");

        var elementContext = await GetElementContext(context).ConfigureAwait(false);
        if (elementContext == null)
        {
            return;
        }

        foreach (var validator in _propertyValidators)
        {
            if (!validator.ExecutionModes.Contains(currentMode))
            {
                continue;
            }

            for (var i = 0; i < elementContext.Elements.Count; i++)
            {
                elementContext.ElementIndex = i;
                await validator.Validate(elementContext, currentMode).ConfigureAwait(false);
                if (!CanExecute(context, currentMode))
                {
                    return;
                }
            }
        }
    }

    private ValueTask<IElementContext<TEntity, TProperty>?> GetElementContext(
        IPropertyValidationContext<TEntity, IEnumerable<TProperty>?> context)
    {
        var value = context.GetValidationMetadata(_metadataId);
        if (value != null)
        {
            var result = ReferenceEquals(value, Constants.FalseObjectValue) ? null : value;

            return ValueTask.FromResult((IElementContext<TEntity, TProperty>?)result);
        }

        return CreateElementContext(context);
    }

    private async ValueTask<IElementContext<TEntity, TProperty>?> CreateElementContext(
        IPropertyValidationContext<TEntity, IEnumerable<TProperty>?> context)
    {
        var propertyValue = context.PropertyValue;
        if (propertyValue == null)
        {
            context.SetValidationMetadata(_metadataId, Constants.FalseObjectValue);

            return null;
        }

        var elements = _condition != null
            ? await FilterElements(context).ConfigureAwait(false)
            : propertyValue.ToArray();

        if (elements.Count == 0)
        {
            context.SetValidationMetadata(_metadataId, Constants.FalseObjectValue);

            return null;
        }

        var elementContext = _elementContextFactory.Create(context, elements);
        context.SetValidationMetadata(_metadataId, elementContext);

        return elementContext;
    }

    private async ValueTask<IList<TProperty>> FilterElements(
        IPropertyValidationContext<TEntity, IEnumerable<TProperty>?> context)
    {
        var elements = new List<TProperty>();
        foreach (var element in context.PropertyValue!)
        {
            var condition = await _condition!.Invoke(context.ValidationContext, element).ConfigureAwait(false);
            if (condition)
            {
                elements.Add(element);
            }
        }

        return elements;
    }
}