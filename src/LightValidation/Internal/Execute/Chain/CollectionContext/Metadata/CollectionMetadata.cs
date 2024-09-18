using System;
using System.Buffers;
using System.Diagnostics;

namespace LightValidation.Internal.Execute.Chain.CollectionContext.Metadata;

internal interface ICollectionMetadata
{
    object? GetElementMetadata(int elementIndex);

    void SetElementMetadata(int elementIndex, object? value);
}

internal sealed class CollectionMetadata : ICollectionMetadata, IDisposable
{
    private readonly object?[] _metadata;
    private readonly int _elementCount;

    public CollectionMetadata(int elementCount)
    {
        Debug.Assert(elementCount > 0, "Elements count must be greater than zero.");

        _metadata = ArrayPool<object?>.Shared.Rent(elementCount);

        Debug.Assert(!Array.Exists(_metadata, x => x != null), "Rented metadata contains non-null objects.");

        _elementCount = elementCount;
    }

    public object? GetElementMetadata(int elementIndex)
    {
        Debug.Assert(elementIndex < _elementCount, "Index was outside the bounds of the metadata array.");

        return _metadata[elementIndex];
    }

    public void SetElementMetadata(int elementIndex, object? value)
    {
        Debug.Assert(elementIndex < _elementCount, "Index was outside the bounds of the metadata array.");

        _metadata[elementIndex] = value;
    }

    public void Dispose()
    {
        foreach (var value in _metadata)
        {
            if (value is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        Array.Clear(_metadata, 0, _elementCount);
        ArrayPool<object?>.Shared.Return(_metadata);
    }
}
