using LightValidation.Internal.ExpressionNodes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace LightValidation.Internal;

internal sealed class PropertyNode<TError> : IEquatable<PropertyNode<TError>?>
{
    private readonly HashSet<PropertyNode<TError>> _children = [];
    private readonly IReadOnlyList<object?> _constants;
    private readonly PropertyNode<TError> _root;
    private readonly ExpressionNode? _node;

    private bool _isValid = true;
    private object? _validator;

    public PropertyNode()
    {
        _constants = [];
        _root = this;
        _node = null;
    }

    private PropertyNode(IReadOnlyList<object?> constants, PropertyNode<TError> root, ExpressionNode node)
    {
        _constants = constants;
        _root = root;
        _node = node;
    }

    public bool IsRootValid => _root.IsValid;

    public bool IsValid
    {
        get
        {
            if (_isValid)
            {
                _isValid = _children.All(x => x.IsValid);
            }

            return _isValid;
        }
    }

    public IValidator<TValue, TError> GetValidator<TValue>(
        PropertySelectorInfo selectorInfo, Func<PropertyNode<TError>, IValidator<TValue, TError>> validatorFactory)
    {
        var node = GetPropertyNode(selectorInfo.Nodes, selectorInfo.Constants);
        node._validator ??= validatorFactory.Invoke(node);

        return (IValidator<TValue, TError>)node._validator;
    }

    public void SetInvalid()
    {
        _isValid = false;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as PropertyNode<TError>);
    }

    public bool Equals(PropertyNode<TError>? other)
    {
        return other != null && _constants.SequenceEqual(other._constants) && CompareExpressionNodes(_node, other._node);
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        var nodeHashCode = GetExpressionNodeHashCode(_node);
        hashCode.Add(nodeHashCode);

        foreach (var constant in _constants)
        {
            hashCode.Add(constant);
        }

        return hashCode.ToHashCode();
    }

    private static bool CompareExpressionNodes(ExpressionNode? left, ExpressionNode? right)
    {
        return (left, right) switch
        {
            (MethodNode x, ValueNode y) => CompareNodes(x, y),
            (ValueNode x, MethodNode y) => CompareNodes(y, x),
            _ => Equals(left, right),
        };
    }

    private static int GetExpressionNodeHashCode(ExpressionNode? node)
    {
        if (node is ValueNode valueNode && IsIndexer(valueNode))
        {
            return HashCode.Combine(valueNode.ValueType);
        }

        if (node is MethodNode methodNode && IsIndexer(methodNode))
        {
            return HashCode.Combine(methodNode.Method.ReturnType);
        }

        return node?.GetHashCode() ?? 0;
    }

    private static bool CompareNodes(MethodNode left, ValueNode right)
    {
        if (!IsIndexer(left) || !IsIndexer(right))
        {
            return false;
        }

        return left.Method.ReturnType == right.ValueType;
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

    private static IReadOnlyList<object?> GetNodeConstants(
        ExpressionNode node, IReadOnlyList<object?> constants, int constantIndex)
    {
        if (node.ExpressionType == ExpressionType.ArrayIndex)
        {
            return GetSubSequence(constants, constantIndex, 1);
        }

        if (node is MethodNode methodNode)
        {
            var parameterCount = methodNode.Method.GetParameters().Length;

            return GetSubSequence(constants, constantIndex, parameterCount);
        }

        return [];
    }

    private static IReadOnlyList<object?> GetSubSequence(IReadOnlyList<object?> constants, int index, int length)
    {
        if (constants.Count == length)
        {
            Debug.Assert(index == 0);

            return constants;
        }

        if (length == 0)
        {
            return [];
        }

        if (length == 1)
        {
            return [constants[index]];
        }

        var result = new object?[length];
        for (var i = index; i < index + length; i++)
        {
            result[i - index] = constants[i];
        }

        return result;
    }

    private PropertyNode<TError> GetPropertyNode(
        IReadOnlyList<ExpressionNode> nodes, IReadOnlyList<object?> constants, int nodeIndex = 0, int constantIndex = 0)
    {
        var expressionNode = nodes[nodeIndex++];
        if (expressionNode.ExpressionType == ExpressionType.Parameter)
        {
            return nodes.Count > nodeIndex ? GetPropertyNode(nodes, constants, nodeIndex, constantIndex) : this;
        }

        var nodeConstants = GetNodeConstants(expressionNode, constants, constantIndex);
        constantIndex += nodeConstants.Count;

        var propertyNode = new PropertyNode<TError>(nodeConstants, _root, expressionNode);
        if (!_children.TryGetValue(propertyNode, out var existingNode))
        {
            _children.Add(propertyNode);
            existingNode = propertyNode;
        }

        return nodes.Count > nodeIndex ? existingNode.GetPropertyNode(nodes, constants, nodeIndex, constantIndex) : existingNode;
    }
}
