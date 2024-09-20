using LightValidation.Abstractions.Build;
using LightValidation.Constants;
using LightValidation.Extensions;
using LightValidation.SourceGeneration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LightValidation.Rules;

[ExtensionMethod("Satisfy", IsPublic = true)]
public sealed class PredicateRule<TEntity, TProperty> : AsyncRuleBase<TEntity, TProperty>
{
    private readonly Func<TEntity, TProperty, CancellationToken, ValueTask<bool>> _predicate;

    public PredicateRule(Func<TEntity, TProperty, CancellationToken, ValueTask<bool>> predicate)
    {
        _predicate = predicate;
    }

    public PredicateRule(Func<TEntity, TProperty, ValueTask<bool>> predicate)
    {
        _predicate = (entity, property, _) => predicate.Invoke(entity, property);
    }

    public PredicateRule(Func<TProperty, CancellationToken, ValueTask<bool>> predicate)
    {
        _predicate = (_, property, cancellationToken) => predicate.Invoke(property, cancellationToken);
    }

    public PredicateRule(Func<TProperty, ValueTask<bool>> predicate)
    {
        _predicate = (_, property, _) => predicate.Invoke(property);
    }

    public PredicateRule(Func<TEntity, TProperty, bool> predicate)
    {
        _predicate = (entity, property, _) => ValueTask.FromResult(predicate.Invoke(entity, property));
    }

    public PredicateRule(Func<TProperty, bool> predicate)
    {
        _predicate = (_, property, _) => ValueTask.FromResult(predicate.Invoke(property));
    }

    public override void Configure(IRuleBuildContext<TEntity, TProperty> context)
    {
        context
            .WithDefaultErrorCode(ErrorCode.ValueMustSatisfyPredicateCondition)
            .WithDefaultErrorDescription("\"{PropertyName}\" does not satisfy the specified condition");
    }

    public override ValueTask<bool> Validate(ValidationContext<TEntity> context, TProperty value)
    {
        return _predicate.Invoke(context.Entity, value, context.CancellationToken);
    }
}
