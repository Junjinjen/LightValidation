using LightValidation.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace LightValidation.Internal.Build.Extensions;

internal static class LambdaExpressionExtensions
{
    private const char PropertyPathSeparator = '.';

    private static readonly MemberTypes[] AllowedMemberTypes =
    [
        MemberTypes.Property,
        MemberTypes.Field,
    ];

    public static string EnsureValidPropertySelectorExpression(
        this LambdaExpression propertySelectorExpression,
        [CallerArgumentExpression(nameof(propertySelectorExpression))] string? paramName = default)
    {
        ArgumentNullException.ThrowIfNull(propertySelectorExpression);

        var path = new List<string>();
        if (!UnwindExpression(propertySelectorExpression.Body, path))
        {
            throw new ArgumentException(
                "Property selector expression must consist only of member access operations.", paramName);
        }

        if (path.Count == 0)
        {
            return RootProperty.Name;
        }

        path.Reverse();

        return string.Join(PropertyPathSeparator, path);
    }

    private static bool UnwindExpression(Expression? expression, List<string> path)
    {
        if (expression == null)
        {
            return false;
        }

        if (expression is MemberExpression memberExpression)
        {
            var member = memberExpression.Member;
            if (!AllowedMemberTypes.Contains(member.MemberType))
            {
                return false;
            }

            path.Add(member.Name);

            return UnwindExpression(memberExpression.Expression, path);
        }

        return expression.NodeType == ExpressionType.Parameter;
    }
}
