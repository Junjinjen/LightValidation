using System;
using System.Linq.Expressions;
using System.Reflection;

namespace LightValidation.Internal.ExpressionNodes;

internal sealed class MemberNode : ExpressionNode, IEquatable<MemberNode>
{
    public MemberNode(MemberInfo member)
        : base(ExpressionType.MemberAccess)
    {
        Member = member;
    }

    public MemberInfo Member { get; }

    public override bool Equals(object? obj)
    {
        return Equals(obj as MemberNode);
    }

    public bool Equals(MemberNode? other)
    {
        return base.Equals(other) && Member.Equals(other.Member);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Member);
    }
}
