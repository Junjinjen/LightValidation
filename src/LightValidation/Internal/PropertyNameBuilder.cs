using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace LightValidation.Internal;

internal sealed class PropertyNameBuilder : ExpressionVisitor
{
    private readonly IPropertyNameResolver _nameResolver;

    private readonly StringBuilder _builder = new();

    private PropertyNameBuilder(IPropertyNameResolver nameResolver)
    {
        _nameResolver = nameResolver;
    }

    public static string Build(LambdaExpression pathExpression, IPropertyNameResolver nameResolver)
    {
        var visitor = new PropertyNameBuilder(nameResolver);
        visitor.Visit(pathExpression.Body);

        return visitor._builder.ToString();
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        Visit(node.Expression);

        var name = _nameResolver.GetMemberName(node.Member);
        Append(name);

        return node;
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        Visit(node.Object);
        if (!node.Method.IsSpecialName)
        {
            var name = _nameResolver.GetMethodName(node.Method);
            if (!Append(name))
            {
                return node;
            }
        }

        var format = node.Method.IsSpecialName ? _nameResolver.GetIndexFormat() : _nameResolver.GetArgumentsFormat();
        Append(format, node.Arguments);

        return node;
    }

    protected override Expression VisitUnary(UnaryExpression node)
    {
        Debug.Assert(node.NodeType is ExpressionType.ArrayLength or ExpressionType.Convert);

        if (node.NodeType == ExpressionType.ArrayLength)
        {
            Append(nameof(Array.Length));
        }

        return node;
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {
        Debug.Assert(node.NodeType == ExpressionType.ArrayIndex);

        Visit(node.Left);

        var format = _nameResolver.GetIndexFormat();
        Append(format, node.Right);

        return node;
    }

    private static string? ConvertToString(object? value)
    {
        if (value is string stringValue)
        {
            return $"\"{stringValue}\"";
        }

        if (value is char charValue)
        {
            return $"'{charValue}'";
        }

        return value?.ToString();
    }

    private bool Append(string? name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return false;
        }

        if (_builder.Length > 0)
        {
            _builder.Append('.');
        }

        _builder.Append(name);

        return true;
    }

    private void Append(CompositeFormat? format, params IReadOnlyList<Expression> argumentExpressions)
    {
        if (format == null)
        {
            return;
        }

        var arguments = argumentExpressions.Cast<ConstantExpression>().Select(x => ConvertToString(x.Value));
        var argumentsValue = string.Join(", ", arguments);
        var formatted = string.Format(CultureInfo.InvariantCulture, format, argumentsValue);
        _builder.Append(formatted);
    }
}
