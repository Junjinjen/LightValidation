using LightValidation.Abstractions.Execute;

namespace LightValidation.Abstractions.Build;

public interface IRuleBuilder<TEntity, in TProperty>
{
    IPropertyRule<TEntity, TProperty> Build(IRuleBuildContext<TEntity, TProperty> context);
}
