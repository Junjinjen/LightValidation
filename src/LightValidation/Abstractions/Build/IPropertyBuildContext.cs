using LightValidation.Abstractions.Execute;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LightValidation.Abstractions.Build;

public interface IPropertyBuildContext
{
    IEntityBuildContext EntityBuildContext { get; }

    Type ValidatorType { get; }

    IReadOnlyDictionary<Type, ExecutionMode> ExecutionModeByAttribute { get; }

    ExecutionMode DefaultExecutionMode { get; }

    LambdaExpression PropertySelectorExpression { get; }

    Delegate PropertySelector { get; }

    string PropertyName { get; }

    int RegisterMetadata();
}
