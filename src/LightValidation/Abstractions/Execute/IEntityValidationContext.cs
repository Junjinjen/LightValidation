using LightValidation.Result;
using System;

namespace LightValidation.Abstractions.Execute;

public interface IEntityValidationContext<TEntity> : IDisposable
{
    ValidationContext<TEntity> ValidationContext { get; }

    bool IsEntityValid { get; }

    object? GetValidationMetadata(int metadataId);

    void SetValidationMetadata(int metadataId, object? value);

    void AddRuleFailure(RuleFailure failure);
}
