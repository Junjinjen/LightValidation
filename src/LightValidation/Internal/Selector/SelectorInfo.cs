using System.Collections.Generic;
using System.Linq.Expressions;

namespace LightValidation.Internal.Selector;

internal class SelectorInfo
{
    public required object Key { get; init; }

    public required Expression Body { get; init; }

    public required IReadOnlyList<object?> Constants { get; init; }
}
