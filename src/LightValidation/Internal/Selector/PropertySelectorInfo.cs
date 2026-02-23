using System.Linq.Expressions;

namespace LightValidation.Internal.Selector;

internal sealed class PropertySelectorInfo : SelectorInfo
{
    public required ParameterExpression ModelParameter { get; init; }

    public required string PropertyPath { get; init; }
}
