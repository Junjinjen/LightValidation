using LightValidation.Abstractions.Execute;
using LightValidation.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LightValidation;

[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(CollectionDebugView<ExecutionMode>))]
public readonly struct ExecutionModeCollection
    : IReadOnlyCollection<ExecutionMode>, IEquatable<ExecutionModeCollection>
{
    private const int OnValidPropertyValue = 2;
    private const int OnValidEntityValue = 4;
    private const int AlwaysValue = 1;

    private readonly int _data;

    public ExecutionModeCollection(IEnumerable<ExecutionMode> modes)
    {
        var values = modes.Select(GetFlagValue);
        foreach (var value in values)
        {
            _data |= value;
        }
    }

    public int Count => GetCount();

    public static bool operator ==(ExecutionModeCollection left, ExecutionModeCollection right)
    {
        return left._data == right._data;
    }

    public static bool operator !=(ExecutionModeCollection left, ExecutionModeCollection right)
    {
        return left._data != right._data;
    }

    public bool Contains(ExecutionMode mode)
    {
        var value = GetFlagValue(mode);

        return HasFlag(value);
    }

    public override bool Equals(object? obj)
    {
        return obj is ExecutionModeCollection collection && Equals(collection);
    }

    public bool Equals(ExecutionModeCollection other)
    {
        return _data == other._data;
    }

    public override int GetHashCode()
    {
        return _data.GetHashCode();
    }

    public IEnumerator<ExecutionMode> GetEnumerator()
    {
        if (HasFlag(AlwaysValue))
        {
            yield return ExecutionMode.Always;
        }

        if (HasFlag(OnValidPropertyValue))
        {
            yield return ExecutionMode.OnValidProperty;
        }

        if (HasFlag(OnValidEntityValue))
        {
            yield return ExecutionMode.OnValidEntity;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private static int GetFlagValue(ExecutionMode mode)
    {
        if (mode == ExecutionMode.Always)
        {
            return AlwaysValue;
        }

        if (mode == ExecutionMode.OnValidProperty)
        {
            return OnValidPropertyValue;
        }

        Debug.Assert(mode == ExecutionMode.OnValidEntity, $"Execution mode value \"{mode}\" is invalid.");

        return OnValidEntityValue;
    }

    private bool HasFlag(int flagValue)
    {
        return (_data & flagValue) == flagValue;
    }

    private int GetCount()
    {
        var result = 0;
        if (HasFlag(AlwaysValue))
        {
            result++;
        }

        if (HasFlag(OnValidPropertyValue))
        {
            result++;
        }

        if (HasFlag(OnValidEntityValue))
        {
            result++;
        }

        return result;
    }
}
