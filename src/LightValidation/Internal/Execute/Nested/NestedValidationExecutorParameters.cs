using System;
using System.Threading.Tasks;

namespace LightValidation.Internal.Execute.Nested;

internal readonly ref struct NestedValidationExecutorParameters<TEntity, TProperty>
{
    public readonly Func<ValidationContext<TEntity>, IValidatorInternal<TProperty>> ValidatorProvider { get; init; }

    public readonly Func<ValidationContext<TEntity>, TProperty, ValueTask<bool>>? Condition { get; init; }

    public readonly string PropertyName { get; init; }

    public readonly int MetadataId { get; init; }
}
