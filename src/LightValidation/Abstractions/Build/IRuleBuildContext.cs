using LightValidation.Abstractions.Execute;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LightValidation.Abstractions.Build;

public interface IRuleBuildContext<TEntity, out TProperty>
{
    Type ValidatorType { get; }

    LambdaExpression PropertySelectorExpression { get; }

    Delegate PropertySelector { get; }

    IErrorMetadataProvider CreateErrorMetadataProvider(string key);

    void AddCondition(Func<ValidationContext<TEntity>, TProperty, ValueTask<bool>> condition);

    void AppendCollectionIndexToPropertyName(bool value);

    void SetDefaultErrorCode(string defaultErrorCode);

    void SetDefaultErrorDescription(string defaultErrorDescription);

    void AddErrorMetadata(string key, object? value);

    void AddErrorMetadata(string key, Func<ValidationContext<TEntity>, TProperty, object?> valueSelector);

    void SetErrorMetadataLocalization(string key, Func<object?, string> localizer);
}
