using System;
using System.Linq.Expressions;

namespace LightValidation.Abstractions.Build;

public interface IPropertyBuildContext : IExecutionModeContext
{
    IEntityBuildContext EntityContext { get; }

    Type ValidatorType { get; }

    LambdaExpression PropertySelectorExpression { get; }

    Delegate PropertySelector { get; }

    string PropertyName { get; }

    int RegisterValidationMetadata();
}
