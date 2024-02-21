using LightValidation.Abstractions.Execute;

namespace LightValidation.Abstractions.Build;

public interface IRule<TEntity, in TProperty> : IPropertyRule<TEntity, TProperty>
{
    void Configure(IRuleBuildContext<TEntity, TProperty> context);
}
