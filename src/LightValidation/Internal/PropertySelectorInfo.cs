using System.Linq.Expressions;

namespace LightValidation.Internal;

internal sealed class PropertySelectorInfo : SelectorInfo
{
    public required ParameterExpression ModelParameter { get; init; }
}
