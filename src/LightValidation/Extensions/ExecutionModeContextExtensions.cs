using LightValidation.Abstractions.Build;
using LightValidation.Abstractions.Execute;
using System;
using System.Linq;

namespace LightValidation.Extensions;

public static class ExecutionModeContextExtensions
{
    public static ExecutionMode GetExecutionMode(this IExecutionModeContext context, Type validatorType)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(validatorType);

        var attributeType = context.ExecutionModeByAttribute.Keys.SingleOrDefault(
            x => Attribute.GetCustomAttribute(validatorType, x) != null);

        return attributeType != null ? context.ExecutionModeByAttribute[attributeType] : context.DefaultExecutionMode;
    }

    public static ExecutionMode GetExecutionMode<TValidator>(this IExecutionModeContext context)
    {
        return context.GetExecutionMode(typeof(TValidator));
    }
}
