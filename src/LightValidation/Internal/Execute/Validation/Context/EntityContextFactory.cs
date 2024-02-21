using LightValidation.Abstractions.Execute;
using LightValidation.Result;
using System.Collections.Generic;

namespace LightValidation.Internal.Execute.Validation.Context;

internal interface IEntityContextFactory
{
    IEntityValidationContext<TEntity> Create<TEntity>(
        List<RuleFailure> brokenRules, int metadataCount, ValidationContext<TEntity> context);
}

internal sealed class EntityContextFactory : IEntityContextFactory
{
    public IEntityValidationContext<TEntity> Create<TEntity>(
        List<RuleFailure> brokenRules, int metadataCount, ValidationContext<TEntity> context)
    {
        return new EntityContext<TEntity>(brokenRules, metadataCount)
        {
            ValidationContext = context,
        };
    }
}
