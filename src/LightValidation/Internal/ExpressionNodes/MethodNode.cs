using System;
using System.Linq.Expressions;
using System.Reflection;

namespace LightValidation.Internal.ExpressionNodes;

internal sealed class MethodNode : ExpressionNode, IEquatable<MethodNode>
{
    public MethodNode(MethodInfo method)
        : base(ExpressionType.Call)
    {
        Method = method;
    }

    public MethodInfo Method { get; }

    public override bool Equals(object? obj)
    {
        return Equals(obj as MethodNode);
    }

    public bool Equals(MethodNode? other)
    {
        return base.Equals(other) && Method.Equals(other.Method);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Method);
    }
}
