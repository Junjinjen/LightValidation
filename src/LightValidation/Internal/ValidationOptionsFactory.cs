namespace LightValidation.Internal;

internal interface IValidationOptionsFactory
{
    IValidationOptionsInternal Create();
}

internal sealed class ValidationOptionsFactory : IValidationOptionsFactory
{
    public IValidationOptionsInternal Create()
    {
        return new ValidationOptions();
    }
}
