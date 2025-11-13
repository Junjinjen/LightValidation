using LightValidation.Internal.ExpressionNodes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LightValidation.Internal;

internal static class SelectorCache
{
    private static readonly ConcurrentDictionary<CacheKey, Func<IReadOnlyList<object?>, object?>> ArgumentSelectorCache = [];
    private static readonly ConcurrentDictionary<CacheKey, Delegate> PropertySelectorCache = [];

    public static Func<TModel, TProperty> GetPropertySelector<TModel, TProperty>(PropertySelectorInfo selectorInfo)
    {
        var key = new CacheKey(selectorInfo.Nodes);
        var selector = PropertySelectorCache.GetOrAdd(key, _ =>
        {
            var body = selectorInfo.Body;
            LambdaExpression lambda = selectorInfo.Constants.Count > 0
                ? Expression.Lambda<Func<TModel, IReadOnlyList<object?>, TProperty>>(
                    body, selectorInfo.ModelParameter, selectorInfo.ConstantsParameter)
                : Expression.Lambda<Func<TModel, TProperty>>(body, selectorInfo.ModelParameter);

            return lambda.Compile();
        });

        if (selectorInfo.Constants.Count == 0)
        {
            return (Func<TModel, TProperty>)selector;
        }

        var typed = (Func<TModel, IReadOnlyList<object?>, TProperty>)selector;
        var constants = selectorInfo.Constants;

        return x => typed.Invoke(x, constants);
    }

    public static Func<object?> GetArgumentSelector(SelectorInfo selectorInfo)
    {
        var key = new CacheKey(selectorInfo.Nodes);
        var selector = ArgumentSelectorCache.GetOrAdd(key, _ =>
        {
            var body = Expression.Convert(selectorInfo.Body, typeof(object));
            var lambda = Expression.Lambda<Func<IReadOnlyList<object?>, object?>>(body, selectorInfo.ConstantsParameter);

            return lambda.Compile();
        });

        var constants = selectorInfo.Constants;

        return () => selector.Invoke(constants);
    }

    private sealed class CacheKey : IEquatable<CacheKey>
    {
        private readonly IReadOnlyList<ExpressionNode> _nodes;

        public CacheKey(IReadOnlyList<ExpressionNode> nodes)
        {
            _nodes = nodes;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as CacheKey);
        }

        public bool Equals(CacheKey? other)
        {
            return other != null && _nodes.SequenceEqual(other._nodes);
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            foreach (var node in _nodes)
            {
                hashCode.Add(node);
            }

            return hashCode.ToHashCode();
        }
    }
}
