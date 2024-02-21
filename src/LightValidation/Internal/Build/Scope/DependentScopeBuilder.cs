using LightValidation.Abstractions.Build;
using LightValidation.Abstractions.Execute;
using LightValidation.Internal.Build.Extensions;
using LightValidation.Internal.Execute.Scope;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LightValidation.Internal.Build.Scope;

internal sealed class DependentScopeBuilder<TEntity> : BuilderBase, IScopeBuilder<TEntity>
{
    private readonly IDependentScopeFactory _dependentScopeFactory;

    private readonly List<IEntityValidatorBuilder<TEntity>> _entityValidatorBuilders = [];

    public DependentScopeBuilder(IDependentScopeFactory dependentScopeFactory)
    {
        _dependentScopeFactory = dependentScopeFactory;
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
        if (entityValidators.Length <= 1)
        {
            return entityValidators.SingleOrDefault();
        }

        return _dependentScopeFactory.Create(entityValidators);
    }
}
