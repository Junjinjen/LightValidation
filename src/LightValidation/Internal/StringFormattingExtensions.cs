using System;

namespace LightValidation.Internal;

internal static class StringFormattingExtensions
{
    public static bool ContainsMetadataKey(this string description, string key)
    {
        return description.GetMetadataKeyIndex(key) >= 0;
    }

    public static int GetMetadataKeyIndex(this string description, string key)
    {
        return description.IndexOf($"{{{key}}}", StringComparison.OrdinalIgnoreCase);
    }

    public static string Format(this string description, string key, object? value)
    {
        return description.Replace($"{{{key}}}", value?.ToString(), StringComparison.OrdinalIgnoreCase);
    }
}
