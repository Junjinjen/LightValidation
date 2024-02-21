using LightValidation.Abstractions.Execute;

namespace LightValidation.Abstractions.Build;

public interface IPropertyValidatorBuilder<TEntity, in TProperty>
{
    IPropertyValidator<TEntity, TProperty>? Build(IPropertyBuildContext context);
}
