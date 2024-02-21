using LightValidation.Internal.Build.Extensions;
using System;

namespace LightValidation.Internal.Build.Rule.FailureGeneration;

internal interface IDefaultRuleConfiguration
{
    string GetErrorCode(Type ruleType);

    string GetErrorDescription(Type ruleType);
}

internal sealed class DefaultRuleConfiguration : IDefaultRuleConfiguration
{
    private const string RuleBuilderPostfix = "RuleBuilder";
    private const string RulePostfix = "Rule";

    public string GetErrorCode(Type ruleType)
    {
        var ruleName = ruleType.GetNameWithoutGenericInfo();
        ruleName = GetNameWithoutPostfix(ruleName, RuleBuilderPostfix);
        ruleName = GetNameWithoutPostfix(ruleName, RulePostfix);

        return $"{ruleName}_rule_failed";
    }

    public string GetErrorDescription(Type ruleType)
    {
        var ruleName = ruleType.GetNameWithoutGenericInfo();
        ruleName = GetNameWithoutPostfix(ruleName, RuleBuilderPostfix);
        ruleName = GetNameWithoutPostfix(ruleName, RulePostfix);

        return $"\"{{{MetadataKey.PropertyName}}}\" violated the \"{ruleName}\" rule";
    }

    private static string GetNameWithoutPostfix(string ruleName, string postfix)
    {
        return ruleName.Length > postfix.Length && ruleName.EndsWith(postfix, StringComparison.OrdinalIgnoreCase)
            ? ruleName[..^postfix.Length]
            : ruleName;
    }
}
