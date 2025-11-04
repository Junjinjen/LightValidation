using System;
using System.Linq.Expressions;

namespace LightValidation.Internal.ExpressionNodes;

internal sealed class ValueNode : ExpressionNode, IEquatable<ValueNode>
{
    public ValueNode(ExpressionType expressionType, Type valueType)
        : base(expressionType)
    {
        ValueType = valueType;
    }

    public Type ValueType { get; }

    public override bool Equals(object? obj)
    {
        return Equals(obj as ValueNode);
    }

    public bool Equals(ValueNode? other)
    {
        return base.Equals(other) && ValueType.Equals(other.ValueType);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), ValueType);
    }
}
