using LightValidation.Abstractions.Execute;

namespace LightValidation.Abstractions.Build;

public interface IEntityValidatorBuilder<TEntity>
{
    IEntityValidator<TEntity>? Build(IEntityBuildContext context);
}
