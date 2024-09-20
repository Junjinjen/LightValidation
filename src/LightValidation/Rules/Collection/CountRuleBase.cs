using LightValidation.Abstractions.Build;
using LightValidation.Abstractions.Execute;
using System;
using System.Collections;

namespace LightValidation.Rules.Collection;

public abstract class CountRuleBase<TEntity, TProperty> : RuleBase<TEntity, TProperty>
    where TProperty : IEnumerable
{
    private IErrorMetadataProvider _actualCountProvider = null!;

    public override void Configure(IRuleBuildContext<TEntity, TProperty> context)
    {
        _actualCountProvider = context.CreateErrorMetadataProvider("ActualCount");
    }

    protected int GetCount(TProperty value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value is ICollection collection)
        {
            var result = collection.Count;
            _actualCountProvider.SetValue(result);

            return result;
        }

        var enumerator = value.GetEnumerator();
        using (enumerator as IDisposable)
        {
            var result = 0;
            while (enumerator.MoveNext())
            {
                result++;
            }

            _actualCountProvider.SetValue(result);

            return result;
        }
    }
}
