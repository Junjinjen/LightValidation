using LightValidation.Abstractions.Build;
using LightValidation.Abstractions.Execute;

namespace LightValidation.EntityFramework;

public abstract class DatabaseValidatorBase<TEntity> : ValidatorBase<TEntity>
{
    protected override void OnValidationBuild(IValidationBuilder<TEntity> builder)
    {
        builder.SetExecutionModeForAttribute(typeof(DatabaseRuleAttribute), ExecutionMode.OnValidEntity);
    }
}
