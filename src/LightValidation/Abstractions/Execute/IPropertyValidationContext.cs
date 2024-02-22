using LightValidation.Result;
using System;
using System.Text;

namespace LightValidation.Abstractions.Execute;

public interface IPropertyValidationContext<TEntity, out TProperty>
{
    IEntityValidationContext<TEntity> EntityValidationContext { get; }

    ValidationContext<TEntity> ValidationContext { get; }

    TProperty PropertyValue { get; }

    bool IsPropertyValid { get; }

    bool IsEntityValid { get; }

    bool CanExecuteDependentRules { get; }

    Action<StringBuilder>? CollectionIndexBuilder { get; }

    object? GetValidationMetadata(int metadataId);

    void SetValidationMetadata(int metadataId, object? value);

    void AddRuleFailure(RuleFailure failure);
}
