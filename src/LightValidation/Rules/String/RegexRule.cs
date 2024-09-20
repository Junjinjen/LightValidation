using LightValidation.Abstractions.Build;
using LightValidation.Constants;
using LightValidation.Extensions;
using LightValidation.SourceGeneration;
using System;
using System.Text.RegularExpressions;

namespace LightValidation.Rules.String;

[ExtensionMethod("Match", IsPublic = true)]
public sealed class RegexRule<TEntity> : RuleBase<TEntity, string>
{
    private readonly Regex _regex;

    public RegexRule(Regex regex)
    {
        ArgumentNullException.ThrowIfNull(regex);

        _regex = regex;
    }

    public RegexRule(string pattern, RegexOptions options = RegexOptions.None)
    {
        ArgumentException.ThrowIfNullOrEmpty(pattern);

        _regex = new Regex(pattern, options | RegexOptions.Compiled);
    }

    public override void Configure(IRuleBuildContext<TEntity, string> context)
    {
        context
            .WithDefaultErrorCode(ErrorCode.String.MustMatchRegexPattern)
            .WithDefaultErrorDescription(
                "\"{PropertyName}\" must match the specified regular expression pattern: {Pattern}")
            .WithErrorMetadata("Pattern", _regex);
    }

    public override bool Validate(ValidationContext<TEntity> context, string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return _regex.IsMatch(value);
    }
}
