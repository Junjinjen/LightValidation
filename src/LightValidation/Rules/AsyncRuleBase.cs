using LightValidation.Abstractions.Build;
using System.Threading.Tasks;

namespace LightValidation.Rules;

public abstract class AsyncRuleBase<TEntity, TProperty> : IRule<TEntity, TProperty>
{
    public virtual void Configure(IRuleBuildContext<TEntity, TProperty> context)
    {
    }

    public abstract ValueTask<bool> Validate(ValidationContext<TEntity> context, TProperty value);
}
