using LightValidation.Internal.ExpressionNodes;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LightValidation.Internal;

internal class SelectorInfo
{
    public required ParameterExpression ConstantsParameter { get; init; }

    public required Expression Body { get; init; }

    public required IReadOnlyList<ExpressionNode> Nodes { get; init; }

    public required IReadOnlyList<object?> Constants { get; init; }
}
