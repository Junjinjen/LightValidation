using LightValidation.Abstractions.Execute;
using LightValidation.Internal.Execute.Validation;

namespace LightValidation.Internal.Execute.Nested.Context;

internal readonly ref struct NestedContextParameters<TEntity, TProperty>
{
    public required int MetadataCount { get; init; }

    public required string PropertyName { get; init; }

    public required IPropertyValidationContext<TEntity, TProperty> PropertyContext { get; init; }

    public required ValidationContext<TProperty> ValidationContext { get; init; }

    public required IValidationExecutor<TProperty> ValidationExecutor { get; init; }
}
