using LightValidation.Abstractions.Execute;
using System;
using System.Threading.Tasks;

namespace LightValidation.Abstractions.Build;

public interface IRuleChainConfiguration<TEntity, out TProperty> : IRuleChainBuilder<TEntity, TProperty>
{
    void AddCondition(Func<ValidationContext<TEntity>, TProperty, ValueTask<bool>> condition);

    void SetExecutionModeForAttribute(Type attributeType, ExecutionMode mode);

    void SetDefaultExecutionMode(ExecutionMode mode);

    void SetPropertyName(string propertyName);
}
