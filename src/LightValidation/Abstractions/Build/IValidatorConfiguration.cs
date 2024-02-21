using System;
using System.Threading.Tasks;

namespace LightValidation.Abstractions.Build;

public interface IValidatorConfiguration<TEntity, out TProperty> : IRuleChainBuilder<TEntity, TProperty>
{
    void AddCondition(Func<ValidationContext<TEntity>, TProperty, ValueTask<bool>> condition);

    void SetPropertyName(string propertyName);
}
