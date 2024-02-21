using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LightValidation.Internal;

internal sealed class CollectionDebugView<T>
{
    public CollectionDebugView(IEnumerable<T> enumerable)
    {
        Items = enumerable.ToArray();
    }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public T[] Items { get; }
}
