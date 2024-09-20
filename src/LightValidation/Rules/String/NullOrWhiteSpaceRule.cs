using LightValidation.Abstractions.Build;
using LightValidation.Constants;
using LightValidation.Extensions;
using LightValidation.SourceGeneration;

namespace LightValidation.Rules.String;

[ExtensionMethod("NullOrWhiteSpace", SavePropertyNullability = true, IsPublic = true)]
public sealed class NullOrWhiteSpaceRule<TEntity> : RuleBase<TEntity, string?>
{
    public override void Configure(IRuleBuildContext<TEntity, string?> context)
    {
        context
            .WithDefaultErrorCode(ErrorCode.String.MustBeNullOrWhiteSpace)
            .WithDefaultErrorDescription("\"{PropertyName}\" must be null, empty, or contain only whitespace");
    }

    public override bool Validate(ValidationContext<TEntity> context, string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }
}
