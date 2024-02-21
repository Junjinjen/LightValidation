using LightValidation.Abstractions.Execute;
using System;
using System.Linq.Expressions;

namespace LightValidation.Abstractions.Build;

public interface IValidationBuilder<TEntity>
{
    void AllowNullEntity(bool value);

    void SetNullEntityErrorCode(string errorCode);

    void SetNullEntityErrorDescription(string errorDescription);

    void SetExecutionModeForAttribute(Type attributeType, ExecutionMode mode);

    void SetDefaultExecutionMode(ExecutionMode mode);

    void SetPropertyName(string propertyPath, string propertyName);

    void AddEntityValidator(IEntityValidatorBuilder<TEntity> validatorBuilder);

    void AddPropertyValidator<TProperty>(
        Expression<Func<TEntity, TProperty>> propertySelectorExpression,
        IPropertyValidatorBuilder<TEntity, TProperty> validatorBuilder);

    void AddScope(IScopeBuilder<TEntity> scopeBuilder, bool standaloneMode, Action buildAction);

    IScopeTracker RememberCurrentScope();
}
