namespace LightValidation.Abstractions;

public interface IValidationOptions
{
    IValidationOptions IncludeRuleSets(params string[] ruleSets);

    IValidationOptions UseValidationCache(IValidationCache cache);
}
