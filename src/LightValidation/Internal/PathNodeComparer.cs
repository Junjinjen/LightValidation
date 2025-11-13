using LightValidation.Internal.ExpressionNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LightValidation.Internal;

internal sealed class PathNodeComparer : IEqualityComparer<ExpressionNode>
{
    public static readonly PathNodeComparer Instance = new();

    private PathNodeComparer()
    {
    }

    public bool Equals(ExpressionNode? x, ExpressionNode? y)
    {
        return (x, y) switch
        {
            (ValueNode left, MethodNode right) => CompareNodes(left, right),
            (MethodNode left, ValueNode right) => CompareNodes(right, left),
            (MemberNode left, MemberNode right) => CompareMembers(left.Member, right.Member),
            (MethodNode left, MethodNode right) => CompareMethods(left.Method, right.Method),
            _ => Equals(x, y),
        };
    }

    public int GetHashCode(ExpressionNode? obj)
    {
        if (obj is ValueNode valueNode && IsIndexer(valueNode))
        {
            return HashCode.Combine(valueNode.ValueType);
        }

        if (obj is MethodNode methodNode)
        {
            return GetMethodHashCode(methodNode);
        }

        if (obj is MemberNode memberNode)
        {
            return HashCode.Combine(memberNode.Member.Name);
        }

        return obj?.GetHashCode() ?? 0;
    }

    private static int GetMethodHashCode(MethodNode methodNode)
    {
        var method = methodNode.Method;
        if (IsIndexer(methodNode))
        {
            return HashCode.Combine(method.ReturnType);
        }

        var hashCode = new HashCode();
        hashCode.Add(method.Name);
        hashCode.Add(method.ReturnType);

        var parameters = method.GetParameters();
        foreach (var parameter in parameters)
        {
            hashCode.Add(parameter);
        }

        return hashCode.ToHashCode();
    }

    private static bool IsIndexer(ValueNode node)
    {
        return node.ExpressionType == ExpressionType.ArrayIndex;
    }

    private static bool IsIndexer(MethodNode node)
    {
        var method = node.Method;
        if (!method.IsSpecialName)
        {
            return false;
        }

        var parameters = method.GetParameters();

        return parameters.Length == 1 && parameters[0].ParameterType == typeof(int);
    }

    private static bool CompareNodes(ValueNode left, MethodNode right)
    {
        if (!IsIndexer(left) || !IsIndexer(right))
        {
            return false;
        }

        return left.ValueType == right.Method.ReturnType;
    }

    private static bool CompareMethods(MethodInfo left, MethodInfo right)
    {
        if (!CompareMembers(left, right))
        {
            return false;
        }

        var leftParameters = left.GetParameters();
        var rightParameters = right.GetParameters();

        return leftParameters.SequenceEqual(rightParameters);
    }

    private static bool CompareMembers(MemberInfo left, MemberInfo right)
    {
        if (left == right)
        {
            return true;
        }

        var leftDeclaringType = left.DeclaringType!;
        var rightDeclaringType = right.DeclaringType!;

        if (!leftDeclaringType.IsInterface && !rightDeclaringType.IsInterface)
        {
            return false;
        }

        if (!leftDeclaringType.IsAssignableTo(rightDeclaringType) && !rightDeclaringType.IsAssignableTo(leftDeclaringType))
        {
            return false;
        }

        return left.Name == right.Name;
    }
}
