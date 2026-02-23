using System;
using System.Linq.Expressions;
using System.Reflection;

namespace LightValidation.Internal.Selector;

internal readonly struct ExpressionNode : IEquatable<ExpressionNode>
{
    private readonly ExpressionType _expressionType;
    private readonly object _identifier;

    private ExpressionNode(ExpressionType expressionType, object identifier)
    {
        _expressionType = expressionType;
        _identifier = identifier;
    }

    public static ExpressionNode ForParameter(Type type)
    {
        return new(ExpressionType.Parameter, type);
    }

    public static ExpressionNode ForConstant(Type type)
    {
        return new(ExpressionType.Constant, type);
    }

    public static ExpressionNode ForMember(MemberInfo member)
    {
        return new(ExpressionType.MemberAccess, member);
    }

    public static ExpressionNode ForArrayLength()
    {
        return new(ExpressionType.ArrayLength, typeof(int));
    }

    public static ExpressionNode ForArrayIndex(Type type)
    {
        return new(ExpressionType.ArrayIndex, type);
    }

    public static ExpressionNode ForMethodCall(MethodInfo method)
    {
        return new(ExpressionType.Call, method);
    }

    public override bool Equals(object? obj)
    {
        return obj is ExpressionNode node && Equals(node);
    }

    public bool Equals(ExpressionNode other)
    {
        return _expressionType == other._expressionType && _identifier.Equals(other._identifier);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_expressionType, _identifier);
    }
}
