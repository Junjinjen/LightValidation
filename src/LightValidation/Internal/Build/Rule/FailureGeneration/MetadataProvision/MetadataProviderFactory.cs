namespace LightValidation.Internal.Build.Rule.FailureGeneration.MetadataProvision;

internal interface IMetadataProviderFactory
{
    IMetadataProviderInternal Create();
}

internal sealed class MetadataProviderFactory : IMetadataProviderFactory
{
    public IMetadataProviderInternal Create()
    {
        return new MetadataProvider();
    }
}
