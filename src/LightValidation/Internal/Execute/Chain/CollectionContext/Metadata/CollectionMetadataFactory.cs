namespace LightValidation.Internal.Execute.Chain.CollectionContext.Metadata;

internal interface ICollectionMetadataFactory
{
    ICollectionMetadata Create(int elementCount);
}

internal sealed class CollectionMetadataFactory : ICollectionMetadataFactory
{
    public ICollectionMetadata Create(int elementCount)
    {
        return new CollectionMetadata(elementCount);
    }
}
