using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text;

namespace LightValidation.Internal.Selector;

internal sealed class SelectorParser : ExpressionVisitor
{
    private const string ListItemsPropertyName = "Item";

    private readonly List<ExpressionNode> _argumentNodes = [];
    private readonly List<object?> _argumentConstants = [];
    private readonly List<ExpressionNode> _nodes = [];
    private readonly List<object?> _constants = [];
    private readonly StringBuilder _pathBuilder;

    private bool _inArgumentScope;

    private SelectorParser(string parentPropertyPath)
    {
        _pathBuilder = new(parentPropertyPath);
    }

    public static PropertySelectorInfo Parse(LambdaExpression selectorExpression, string parentPropertyPath)
    {
        var parameter = selectorExpression.Parameters[0];
        if (selectorExpression.Body is MemberExpression memberExpression && parameter == memberExpression.Expression)
        {
            return new PropertySelectorInfo
            {
                ModelParameter = parameter,
                PropertyPath = $"{parentPropertyPath}{Constants.PropertyDelimiter}{memberExpression.Member.Name}",
                Key = memberExpression.Member,
                Body = memberExpression,
                Constants = [],
            };
        }

        var visitor = new SelectorParser(parentPropertyPath);
        var body = visitor.Visit(selectorExpression.Body);

        return new PropertySelectorInfo
        {
            ModelParameter = parameter,
            PropertyPath = visitor._pathBuilder.ToString(),
            Key = new ExpressionKey(visitor._nodes),
            Body = body,
            Constants = visitor._constants,
        };
    }

    [return: NotNullIfNotNull(nameof(node))]
    public override Expression? Visit(Expression? node)
    {
        return node?.NodeType switch
        {
            null
            or ExpressionType.Parameter
            or ExpressionType.Constant
            or ExpressionType.MemberAccess
            or ExpressionType.ArrayLength
            or ExpressionType.ArrayIndex
            or ExpressionType.Call => base.Visit(node),
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

        Debug.Assert(_nodes.Count == 0, "Parameter must be the first node.");

        var parameterNode = ExpressionNode.ForParameter(node.Type);
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

        var constantNode = ExpressionNode.ForConstant(node.Type);
        _argumentNodes.Add(constantNode);
        _argumentConstants.Add(node.Value);

        return CreateParameter(node.Type, _argumentConstants.Count - 1);
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        var result = base.VisitMember(node);

        var memberNode = ExpressionNode.ForMember(node.Member);
        AppendNode(memberNode);
        AppendName(node.Member.Name);

        return result;
    }

    protected override Expression VisitUnary(UnaryExpression node)
    {
        Debug.Assert(node.NodeType is ExpressionType.ArrayLength);

        var result = base.VisitUnary(node);

        var lengthNode = ExpressionNode.ForArrayLength();
        AppendNode(lengthNode);
        AppendName(nameof(Array.Length));

        return result;
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {
        Debug.Assert(node.NodeType == ExpressionType.ArrayIndex);

        var left = Visit(node.Left);

        var indexNode = ExpressionNode.ForArrayIndex(node.Type);
        AppendNode(indexNode);
        AppendChar(Constants.OpeningIndexChar);

        var result = node.Update(left, node.Conversion, VisitArgument(node.Right));
        AppendChar(Constants.ClosingIndexChar);

        return result;
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        var left = Visit(node.Object);

        var methodNode = ExpressionNode.ForMethodCall(node.Method);
        AppendNode(methodNode);

        var openingChar = Constants.OpeningIndexChar;
        var closingChar = Constants.ClosingIndexChar;
        if (!node.Method.IsSpecialName)
        {
            AppendName(node.Method.Name);

            openingChar = Constants.OpeningArgumentsChar;
            closingChar = Constants.ClosingArgumentsChar;
        }

        AppendChar(openingChar);
        var result = node.Update(left, VisitArguments(node.Arguments));
        AppendChar(closingChar);

        return result;
    }

    private static UnaryExpression CreateParameter(Type type, int index)
    {
        var indexExpression = Expression.Property(
            Constants.ConstantsParameter, ListItemsPropertyName, Expression.Constant(index));

        return Expression.Convert(indexExpression, type);
    }

    private IEnumerable<Expression> VisitArguments(IEnumerable<Expression> expressions)
    {
        var isFirstArgument = true;
        foreach (var expression in expressions)
        {
            if (isFirstArgument)
            {
                isFirstArgument = false;
            }
            else if (!_inArgumentScope)
            {
                _pathBuilder.Append(Constants.ArgumentsDelimiter);
            }

            yield return VisitArgument(expression);
        }
    }

    private Expression VisitArgument(Expression expression)
    {
        if (_inArgumentScope)
        {
            return Visit(expression);
        }

        if (expression is ConstantExpression constantExpression)
        {
            var expressionValue = constantExpression.Value;
            _constants.Add(expressionValue);
            AppendValue(expressionValue);

            return CreateParameter(expression.Type, _constants.Count - 1);
        }

        _inArgumentScope = true;
        var body = Visit(expression);

        var info = new SelectorInfo
        {
            Key = new ExpressionKey(_argumentNodes),
            Body = body,
            Constants = _argumentConstants,
        };

        var argumentValue = SelectorInvoker.GetArgumentValue(info);
        _inArgumentScope = false;
        _argumentConstants.Clear();
        _argumentNodes.Clear();

        _constants.Add(argumentValue);
        AppendValue(argumentValue);

        return CreateParameter(expression.Type, _constants.Count - 1);
    }

    private void AppendNode(ExpressionNode node)
    {
        var list = _inArgumentScope ? _argumentNodes : _nodes;
        list.Add(node);
    }

    private void AppendName(string name)
    {
        if (_inArgumentScope)
        {
            return;
        }

        _pathBuilder.Append(Constants.PropertyDelimiter);
        _pathBuilder.Append(name);
    }

    private void AppendChar(char value)
    {
        if (_inArgumentScope)
        {
            return;
        }

        _pathBuilder.Append(value);
    }

    private void AppendValue(object? value)
    {
        Debug.Assert(!_inArgumentScope, "Value must be appended outside the argument scope.");

        if (value is string stringValue)
        {
            _pathBuilder.Append('"');
            _pathBuilder.Append(stringValue);
            _pathBuilder.Append('"');
        }
        else if (value is char charValue)
        {
            _pathBuilder.Append('\'');
            _pathBuilder.Append(charValue);
            _pathBuilder.Append('\'');
        }
        else
        {
            _pathBuilder.Append(value);
        }
    }
}
