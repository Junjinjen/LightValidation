using System.Linq.Expressions;

namespace LightValidation.Internal.ExpressionNodes;

internal abstract class ExpressionNode
{
    protected ExpressionNode(ExpressionType expressionType)
    {
        ExpressionType = expressionType;
    }

    public ExpressionType ExpressionType { get; }

    public override bool Equals(object? obj)
    {
        return obj is ExpressionNode other && ExpressionType == other.ExpressionType;
    }

    public override int GetHashCode()
    {
        return ExpressionType.GetHashCode();
    }
}
