using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LightValidation.Internal.Selector;

internal static class SelectorInvoker
{
    private static readonly ConcurrentDictionary<object, Func<IReadOnlyList<object?>, object?>> ArgumentSelectorCache = [];
    private static readonly ConcurrentDictionary<object, Delegate> PropertySelectorCache = [];

    public static TProperty GetPropertyValue<TModel, TProperty>(TModel model, PropertySelectorInfo selectorInfo)
    {
        var selector = PropertySelectorCache.GetOrAdd(selectorInfo.Key, key =>
        {
            MakeKeyPersistant(key);

            var body = selectorInfo.Body;
            LambdaExpression lambda = selectorInfo.Constants.Count > 0
                ? Expression.Lambda<Func<TModel, IReadOnlyList<object?>, TProperty>>(
                    body, selectorInfo.ModelParameter, Constants.ConstantsParameter)
                : Expression.Lambda<Func<TModel, TProperty>>(body, selectorInfo.ModelParameter);

            return lambda.Compile();
        });

        return selectorInfo.Constants.Count != 0
            ? ((Func<TModel, IReadOnlyList<object?>, TProperty>)selector).Invoke(model, selectorInfo.Constants)
            : ((Func<TModel, TProperty>)selector).Invoke(model);
    }

    public static object? GetArgumentValue(SelectorInfo selectorInfo)
    {
        var selector = ArgumentSelectorCache.GetOrAdd(selectorInfo.Key, key =>
        {
            MakeKeyPersistant(key);

            var body = Expression.Convert(selectorInfo.Body, typeof(object));
            var lambda = Expression.Lambda<Func<IReadOnlyList<object?>, object?>>(body, Constants.ConstantsParameter);

            return lambda.Compile();
        });

        return selector.Invoke(selectorInfo.Constants);
    }

    private static void MakeKeyPersistant(object key)
    {
        if (key is ExpressionKey expressionKey)
        {
            expressionKey.MakePersistant();
        }
    }
}
