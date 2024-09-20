using LightValidation.Abstractions.Build;
using LightValidation.Constants;
using LightValidation.Extensions;
using LightValidation.Internal.Build.Extensions;
using LightValidation.SourceGeneration;
using System;

namespace LightValidation.Rules.String;

[ExtensionMethod("Uri", IsPublic = true)]
public sealed class UriRule<TEntity> : RuleBase<TEntity, string?>
{
    private readonly UriKind _uriKind;

    public UriRule(UriKind uriKind = UriKind.RelativeOrAbsolute)
    {
        uriKind.EnsureDefined();

        _uriKind = uriKind;
    }

    public override void Configure(IRuleBuildContext<TEntity, string?> context)
    {
        context
            .WithDefaultErrorCode(ErrorCode.String.MustBeValidUri)
            .WithDefaultErrorDescription("\"{PropertyName}\" must be a valid {ExpectedKind} URI")
            .WithErrorMetadata("ExpectedKind", _uriKind, LocalizeKind);
    }

    public override bool Validate(ValidationContext<TEntity> context, string? value)
    {
        return Uri.IsWellFormedUriString(value, _uriKind);
    }

    private static string LocalizeKind(UriKind uriKind)
    {
        return uriKind switch
        {
            UriKind.Absolute => "absolute",
            UriKind.Relative => "relative",
            UriKind.RelativeOrAbsolute => "relative or absolute",
            _ => throw new ArgumentException($"Uri kind value \"{uriKind}\" is invalid.", nameof(uriKind)),
        };
    }
}
