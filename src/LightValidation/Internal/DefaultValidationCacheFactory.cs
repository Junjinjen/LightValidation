using LightValidation.Abstractions;

namespace LightValidation.Internal;

internal interface IDefaultValidationCacheFactory
{
    IValidationCache Create();
}

internal sealed class DefaultValidationCacheFactory : IDefaultValidationCacheFactory
{
    public IValidationCache Create()
    {
        return new ValidationCache();
    }
}
