using LightValidation.Abstractions.Execute;
using System;
using System.Threading.Tasks;

namespace LightValidation.Abstractions.Build;

public interface IRuleConfiguration<TEntity, out TProperty> : IRuleChainBuilder<TEntity, TProperty>
{
    void AddCondition(Func<ValidationContext<TEntity>, TProperty, ValueTask<bool>> condition);

    void SetExecutionMode(ExecutionMode mode);

    void SetPropertyName(string propertyName);

    void SetErrorCode(string errorCode);

    void SetErrorDescription(string errorDescription);

    void AddErrorMetadata(string key, object? value);

    void AddErrorMetadata(string key, Func<ValidationContext<TEntity>, TProperty, object?> valueSelector);

    void SetMetadataLocalization(string key, Func<object?, string> localizer);

    void AddDependentRules(Action buildAction);
}

public interface IRuleConfiguration<TEntity, out TProperty, TRule> : IRuleConfiguration<TEntity, TProperty>
    where TRule : notnull
{
    TRule Rule { get; }
}
