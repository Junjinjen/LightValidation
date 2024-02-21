using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LightValidation.Internal.Build.Property;

internal interface IPropertyConditionBuilder<TEntity, TProperty>
{
    void AddCondition(Func<ValidationContext<TEntity>, TProperty, ValueTask<bool>> condition);

    Func<ValidationContext<TEntity>, TProperty, ValueTask<bool>>? Build();
}

internal sealed class PropertyConditionBuilder<TEntity, TProperty>
    : BuilderBase, IPropertyConditionBuilder<TEntity, TProperty>
{
    private readonly List<Func<ValidationContext<TEntity>, TProperty, ValueTask<bool>>> _conditions = [];

    public void AddCondition(Func<ValidationContext<TEntity>, TProperty, ValueTask<bool>> condition)
    {
        ArgumentNullException.ThrowIfNull(condition);
        EnsureNotBuilt();

        _conditions.Add(condition);
    }

    public Func<ValidationContext<TEntity>, TProperty, ValueTask<bool>>? Build()
    {
        SetBuilt();

        if (_conditions.Count <= 1)
        {
            return _conditions.SingleOrDefault();
        }

        var conditions = _conditions.ToArray();

        return async (context, value) =>
        {
            foreach (var condition in conditions)
            {
                var result = await condition.Invoke(context, value).ConfigureAwait(false);
                if (!result)
                {
                    return false;
                }
            }

            return true;
        };
    }
}
