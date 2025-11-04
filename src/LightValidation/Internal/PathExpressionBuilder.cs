using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace LightValidation.Internal;

internal sealed class PathExpressionBuilder : ExpressionVisitor
{
    private readonly IReadOnlyList<object?> _constants;
    private readonly Expression _parentPathBody;

    private int _index;

    public PathExpressionBuilder(IReadOnlyList<object?> constants, Expression parentPathBody)
    {
        _constants = constants;
        _parentPathBody = parentPathBody;
    }

    public static LambdaExpression Build(
        LambdaExpression parentPathExpression, LambdaExpression childPathExpression, IReadOnlyList<object?> constants)
    {
        var visitor = new PathExpressionBuilder(constants, parentPathExpression.Body);
        var body = visitor.Visit(childPathExpression.Body);

        return Expression.Lambda(body, parentPathExpression.Parameters);
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        return _parentPathBody;
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {
        Debug.Assert(node.NodeType == ExpressionType.ArrayIndex);

        var left = Visit(node.Left);

        return node.Update(left, node.Conversion, CreateConstant());
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        var left = Visit(node.Object);

        return node.Update(left, node.Arguments.Select(_ => CreateConstant()));
    }

    private ConstantExpression CreateConstant()
    {
        return Expression.Constant(_constants[_index++]);
    }
}
