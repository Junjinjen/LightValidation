namespace LightValidation.Abstractions.Build;

public interface IRuleChainBuilder<TEntity, out TProperty>
{
    IValidationBuilder<TEntity> ValidationBuilder { get; }

    void AddPropertyValidator(IPropertyValidatorBuilder<TEntity, TProperty> validatorBuilder);
}
