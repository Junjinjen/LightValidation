using LightValidation.Abstractions.Build;
using LightValidation.Constants;
using LightValidation.Extensions;
using LightValidation.SourceGeneration;
using System;
using System.Collections;
using System.Collections.Generic;

namespace LightValidation.Rules.Collection;

[ExtensionMethod("NullOrEmpty", SavePropertyNullability = true, IsPublic = true)]
public sealed class NullOrEmptyRule<TEntity, TProperty> : RuleBase<TEntity, TProperty?>
    where TProperty : IEnumerable
{
    public override void Configure(IRuleBuildContext<TEntity, TProperty?> context)
    {
        context
            .WithDefaultErrorCode(ErrorCode.Collection.MustBeNullOrEmpty)
            .WithDefaultErrorDescription("\"{PropertyName}\" must be null or empty");
    }

    public override bool Validate(ValidationContext<TEntity> context, TProperty? value)
    {
        if (EqualityComparer<TProperty>.Default.Equals(value, default))
        {
            return true;
        }

        if (value is ICollection collection)
        {
            return collection.Count == 0;
        }

        var enumerator = value!.GetEnumerator();
        using (enumerator as IDisposable)
        {
            return !enumerator.MoveNext();
        }
    }
}
