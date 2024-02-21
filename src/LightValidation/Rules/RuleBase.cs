using LightValidation.Abstractions.Build;
using LightValidation.Abstractions.Execute;
using System.Threading.Tasks;

namespace LightValidation.Rules;

public abstract class RuleBase<TEntity, TProperty> : IRule<TEntity, TProperty>
{
    public virtual void Configure(IRuleBuildContext<TEntity, TProperty> context)
    {
    }

    public abstract bool Validate(ValidationContext<TEntity> context, TProperty value);

    ValueTask<bool> IPropertyRule<TEntity, TProperty>.Validate(ValidationContext<TEntity> context, TProperty value)
    {
        var result = Validate(context, value);

        return ValueTask.FromResult(result);
    }
}
