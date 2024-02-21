using LightValidation.Abstractions;
using System.Threading;

namespace LightValidation.Internal;

internal readonly ref struct ValidationContextParameters<TEntity>
{
    public required TEntity? Entity { get; init; }

    public required RuleSetCollection RuleSets { get; init; }

    public required IValidationCache ValidationCache { get; init; }

    public required CancellationToken CancellationToken { get; init; }
}
