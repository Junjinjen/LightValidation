namespace LightValidation.Abstractions;

public interface IValidationOptions
{
    IValidationOptions UseCache(ValidationCache cache);

    IValidationOptions IncludeRuleSets(params string[] ruleSets);
}
