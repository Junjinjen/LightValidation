using LightValidation.Internal.ExpressionNodes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace LightValidation.Internal;

internal sealed class SelectorParser : ExpressionVisitor
{
    private const string ListItemsPropertyName = "Item";

    private static readonly ParameterExpression ConstantsParameter
        = Expression.Parameter(typeof(IReadOnlyList<object?>), "constants");

    private readonly List<ExpressionNode> _argumentNodes = [];
    private readonly List<object?> _argumentConstants = [];
    private readonly List<ExpressionNode> _nodes = [];
    private readonly List<object?> _constants = [];

    private bool _inArgumentScope;

    public static PropertySelectorInfo Parse(LambdaExpression selectorExpression)
    {
        var visitor = new SelectorParser();
        var body = visitor.Visit(selectorExpression.Body);

        return new PropertySelectorInfo
        {
            ModelParameter = selectorExpression.Parameters[0],
            ConstantsParameter = ConstantsParameter,
            Body = body,
            Nodes = visitor._nodes,
            Constants = visitor._constants,
        };
    }

    [return: NotNullIfNotNull(nameof(node))]
    public override Expression? Visit(Expression? node)
    {
        return node?.NodeType switch
        {
            null
            or ExpressionType.ArrayIndex
            or ExpressionType.ArrayLength
            or ExpressionType.Call
            or ExpressionType.MemberAccess
            or ExpressionType.Constant
            or ExpressionType.Convert
            or ExpressionType.Parameter => base.Visit(node),
            _ => throw new ArgumentException($"Unsupported selector expression node type '{node.NodeType}'.", nameof(node)),
        };
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        if (_inArgumentScope)
        {
            throw new ArgumentException(
                "Parameter node is not allowed as an argument in method calls within the selector.", nameof(node));
        }

        var parameterNode = new ValueNode(ExpressionType.Parameter, node.Type);
        _nodes.Add(parameterNode);

        return node;
    }

    protected override Expression VisitConstant(ConstantExpression node)
    {
        if (!_inArgumentScope)
        {
            throw new ArgumentException(
                "Constant nodes are only allowed as arguments in method calls within the selector.", nameof(node));
        }

        var constantNode = new ValueNode(ExpressionType.Constant, node.Type);
        _argumentNodes.Add(constantNode);
        _argumentConstants.Add(node.Value);

        return ConvertToParameter(node, _argumentConstants.Count - 1);
    }

    protected override Expression VisitUnary(UnaryExpression node)
    {
        Debug.Assert(node.NodeType is ExpressionType.ArrayLength or ExpressionType.Convert);

        var result = base.VisitUnary(node);
        var valueNode = new ValueNode(node.NodeType, node.Type);
        AddNode(valueNode);

        return result;
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {
        Debug.Assert(node.NodeType == ExpressionType.ArrayIndex);

        var left = Visit(node.Left);
        var indexNode = new ValueNode(ExpressionType.ArrayIndex, node.Type);
        AddNode(indexNode);

        return node.Update(left, node.Conversion, VisitArgument(node.Right));
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        var result = base.VisitMember(node);
        var memberNode = new MemberNode(node.Member);
        AddNode(memberNode);

        return result;
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        var left = Visit(node.Object);
        var methodNode = new MethodNode(node.Method);
        AddNode(methodNode);

        return node.Update(left, node.Arguments.Select(VisitArgument));
    }

    private static UnaryExpression ConvertToParameter(Expression expression, int index)
    {
        var indexExpression = Expression.Property(ConstantsParameter, ListItemsPropertyName, Expression.Constant(index));

        return Expression.Convert(indexExpression, expression.Type);
    }

    private void AddNode(ExpressionNode node)
    {
        var list = _inArgumentScope ? _argumentNodes : _nodes;
        list.Add(node);
    }

    private Expression VisitArgument(Expression expression)
    {
        if (_inArgumentScope)
        {
            return Visit(expression);
        }

        if (expression is ConstantExpression constantExpression)
        {
            _constants.Add(constantExpression.Value);

            return ConvertToParameter(expression, _constants.Count - 1);
        }

        _inArgumentScope = true;
        var body = Visit(expression);

        var selectorInfo = new SelectorInfo
        {
            ConstantsParameter = ConstantsParameter,
            Body = body,
            Nodes = _argumentNodes,
            Constants = _argumentConstants,
        };

        var argumentSelector = SelectorCache.GetArgumentSelector(selectorInfo);
        var argumentValue = argumentSelector.Invoke();
        _constants.Add(argumentValue);

        _inArgumentScope = false;
        _argumentConstants.Clear();
        _argumentNodes.Clear();

        return ConvertToParameter(expression, _constants.Count - 1);
    }
}
