using LightValidation.Abstractions.Build;
using LightValidation.Abstractions.Execute;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LightValidation.Internal.Build.Property.Context;

internal readonly ref struct PropertyContextParameters
{
    public required LambdaExpression PropertySelectorExpression { get; init; }

    public required ExecutionMode DefaultExecutionMode { get; init; }

    public required Delegate PropertySelector { get; init; }

    public required string PropertyName { get; init; }

    public required IReadOnlyDictionary<Type, ExecutionMode> ExecutionModeByAttribute { get; init; }

    public required IEntityBuildContext EntityContext { get; init; }
}
