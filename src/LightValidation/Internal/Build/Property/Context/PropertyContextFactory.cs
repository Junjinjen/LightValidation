using LightValidation.Abstractions.Build;

namespace LightValidation.Internal.Build.Property.Context;

internal interface IPropertyContextFactory
{
    IPropertyBuildContext Create(in PropertyContextParameters parameters);
}

internal sealed class PropertyContextFactory : IPropertyContextFactory
{
    public IPropertyBuildContext Create(in PropertyContextParameters parameters)
    {
        return new PropertyContext(
            parameters.PropertySelectorExpression,
            parameters.DefaultExecutionMode,
            parameters.PropertySelector,
            parameters.PropertyName,
            parameters.ExecutionModeByAttribute,
            parameters.EntityBuildContext);
    }
}
