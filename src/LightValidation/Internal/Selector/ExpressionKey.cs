using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace LightValidation.Internal.Selector;

internal sealed class ExpressionKey : IEquatable<ExpressionKey>
{
    private IReadOnlyList<ExpressionNode> _nodes;

    public ExpressionKey(List<ExpressionNode> nodes)
    {
        _nodes = nodes;
    }

    public void MakePersistent()
    {
        _nodes = _nodes.ToArray();
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as ExpressionKey);
    }

    public bool Equals(ExpressionKey? other)
    {
        return other != null && _nodes.SequenceEqual(other._nodes);
    }

    [SuppressMessage("Reliability", "S2328:'GetHashCode' should not reference mutable fields",
        Justification = "Collection is cloned as a read-only array without modifications.")]
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        foreach (var node in _nodes)
        {
            hashCode.Add(node);
        }

        return hashCode.ToHashCode();
    }
}
