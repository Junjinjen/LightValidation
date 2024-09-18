namespace LightValidation.Internal.Build.Rule.FailureGeneration.MetadataProvision;

internal interface IErrorMetadataProviderFactory
{
    IErrorMetadataProviderInternal Create();
}

internal sealed class ErrorMetadataProviderFactory : IErrorMetadataProviderFactory
{
    public IErrorMetadataProviderInternal Create()
    {
        return new ErrorMetadataProvider();
    }
}
