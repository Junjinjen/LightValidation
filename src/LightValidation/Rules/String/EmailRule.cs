using LightValidation.Abstractions.Build;
using LightValidation.Constants;
using LightValidation.Extensions;
using LightValidation.SourceGeneration;
using System;

namespace LightValidation.Rules.String;

[ExtensionMethod("Email", IsPublic = true)]
public sealed class EmailRule<TEntity> : RuleBase<TEntity, string>
{
    private const char AtSign = '@';

    public override void Configure(IRuleBuildContext<TEntity, string> context)
    {
        context
            .WithDefaultErrorCode(ErrorCode.String.MustBeValidEmail)
            .WithDefaultErrorDescription("\"{PropertyName}\" must be a valid email address");
    }

    public override bool Validate(ValidationContext<TEntity> context, string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var index = value.IndexOf(AtSign);

        return index > 0 && index != value.Length - 1 && index == value.LastIndexOf(AtSign);
    }
}
