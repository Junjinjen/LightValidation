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
        return new PropertyContext
        {
            EntityBuildContext = parameters.EntityBuildContext,
            ExecutionModeByAttribute = parameters.ExecutionModeByAttribute,
            DefaultExecutionMode = parameters.DefaultExecutionMode,
            PropertySelectorExpression = parameters.PropertySelectorExpression,
            PropertySelector = parameters.PropertySelector,
            PropertyName = parameters.PropertyName,
        };
    }
}
