using LightValidation.Abstractions.Execute;
using LightValidation.Result;
using System;
using System.Text;

namespace LightValidation.Internal.Execute.Property.Context;

internal sealed class PropertyContext<TEntity, TProperty> : IPropertyValidationContext<TEntity, TProperty>
{
    public required IEntityValidationContext<TEntity> EntityContext { get; init; }

    public ValidationContext<TEntity> ValidationContext => EntityContext.ValidationContext;

    public required TProperty PropertyValue { get; init; }

    public bool IsPropertyValid { get; private set; } = true;

    public bool IsEntityValid => EntityContext.IsEntityValid;

    public bool CanExecuteDependentRules => true;

    public Action<StringBuilder>? CollectionIndexBuilder => null;

    public object? GetValidationMetadata(int metadataId)
    {
        return EntityContext.GetValidationMetadata(metadataId);
    }

    public void SetValidationMetadata(int metadataId, object? value)
    {
        EntityContext.SetValidationMetadata(metadataId, value);
    }

    public void AddRuleFailure(RuleFailure failure)
    {
        IsPropertyValid = false;

        EntityContext.AddRuleFailure(failure);
    }
}
