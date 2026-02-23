using System;

namespace LightValidation.Internal;

internal static class PropertyNameUtilities
{
    public static bool Equals(string left, string right)
    {
        return left.Equals(right, StringComparison.OrdinalIgnoreCase);
    }

    public static bool StartsWith(string property, string value)
    {
        return property.StartsWith(value, StringComparison.OrdinalIgnoreCase)
            && (property.Length == value.Length
                || property[value.Length] is Constants.PropertyDelimiter or Constants.OpeningIndexChar);
    }
}
