using LightValidation.Abstractions.Build;
using LightValidation.Constants;
using LightValidation.Extensions;
using LightValidation.SourceGeneration;

namespace LightValidation.Rules.String;

[ExtensionMethod("NullOrEmpty", SavePropertyNullability = true, IsPublic = true)]
public sealed class NullOrEmptyRule<TEntity> : RuleBase<TEntity, string?>
{
    public override void Configure(IRuleBuildContext<TEntity, string?> context)
    {
        context
            .WithDefaultErrorCode(ErrorCode.String.MustBeNullOrEmpty)
            .WithDefaultErrorDescription("\"{PropertyName}\" must be null or empty");
    }

    public override bool Validate(ValidationContext<TEntity> context, string? value)
    {
        return string.IsNullOrEmpty(value);
    }
}
