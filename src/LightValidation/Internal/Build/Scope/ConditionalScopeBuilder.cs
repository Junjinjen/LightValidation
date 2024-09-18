using LightValidation.Abstractions.Build;
using LightValidation.Abstractions.Execute;
using LightValidation.Internal.Build.Extensions;
using LightValidation.Internal.Execute.Scope;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LightValidation.Internal.Build.Scope;

internal sealed class ConditionalScopeBuilder<TEntity> : BuilderBase, IScopeBuilder<TEntity>
{
    private readonly IConditionalScopeFactory _conditionalScopeFactory;

    private readonly List<IEntityValidatorBuilder<TEntity>> _entityValidatorBuilders = [];
    private readonly Func<ValidationContext<TEntity>, ValueTask<bool>> _condition;

    public ConditionalScopeBuilder(
        Func<ValidationContext<TEntity>, ValueTask<bool>> condition, IConditionalScopeFactory conditionalScopeFactory)
    {
        _condition = condition;

        _conditionalScopeFactory = conditionalScopeFactory;
    }

    public void AddEntityValidator(IEntityValidatorBuilder<TEntity> validatorBuilder)
    {
        ArgumentNullException.ThrowIfNull(validatorBuilder);
        EnsureNotBuilt();

        _entityValidatorBuilders.Add(validatorBuilder);
    }

    public IEntityValidator<TEntity>? Build(IEntityBuildContext context)
    {
        SetBuilt();

        var entityValidators = _entityValidatorBuilders.Select(x => x.Build(context)).WhereNotNull().ToArray();
        if (entityValidators.Length == 0)
        {
            return null;
        }

        var metadataId = context.RegisterValidationMetadata();

        return _conditionalScopeFactory.Create(_condition, entityValidators, metadataId);
    }
}
