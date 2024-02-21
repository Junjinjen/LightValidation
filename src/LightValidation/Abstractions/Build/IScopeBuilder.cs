namespace LightValidation.Abstractions.Build;

public interface IScopeBuilder<TEntity> : IEntityValidatorBuilder<TEntity>
{
    void AddEntityValidator(IEntityValidatorBuilder<TEntity> validatorBuilder);
}
