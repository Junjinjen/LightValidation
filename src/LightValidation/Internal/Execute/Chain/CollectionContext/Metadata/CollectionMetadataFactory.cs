namespace LightValidation.Internal.Execute.Chain.CollectionContext.Metadata;

internal interface ICollectionMetadataFactory
{
    ICollectionMetadata Create(int elementsCount);
}

internal sealed class CollectionMetadataFactory : ICollectionMetadataFactory
{
    public ICollectionMetadata Create(int elementsCount)
    {
        return new CollectionMetadata(elementsCount);
    }
}
