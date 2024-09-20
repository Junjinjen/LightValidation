using LightValidation.Abstractions.Build;
using LightValidation.Constants;
using LightValidation.Extensions;
using LightValidation.SourceGeneration;

namespace LightValidation.Rules.String;

[ExtensionMethod("NotNullOrWhiteSpace", IsPublic = true)]
public sealed class NotNullOrWhiteSpaceRule<TEntity> : RuleBase<TEntity, string?>
{
    public override void Configure(IRuleBuildContext<TEntity, string?> context)
    {
        context
            .WithDefaultErrorCode(ErrorCode.String.CannotBeNullOrWhiteSpace)
            .WithDefaultErrorDescription("\"{PropertyName}\" cannot be null, empty, or contain only whitespace");
    }

    public override bool Validate(ValidationContext<TEntity> context, string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }
}
