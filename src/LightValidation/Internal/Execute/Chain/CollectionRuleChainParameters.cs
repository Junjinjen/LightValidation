using LightValidation.Abstractions.Execute;
using System;
using System.Threading.Tasks;

namespace LightValidation.Internal.Execute.Chain;

internal readonly ref struct CollectionRuleChainParameters<TEntity, TProperty>
{
    public required Func<ValidationContext<TEntity>, TProperty, ValueTask<bool>>? Condition { get; init; }

    public required IPropertyValidator<TEntity, TProperty>[] PropertyValidators { get; init; }

    public required int MetadataId { get; init; }

    public required Action<CollectionIndexContext<TEntity, TProperty>> IndexBuilder { get; init; }
}
