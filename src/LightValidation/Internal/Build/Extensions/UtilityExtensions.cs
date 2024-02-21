using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace LightValidation.Internal.Build.Extensions;

internal static class UtilityExtensions
{
    private const char GenericInfoSeparator = '`';

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> enumerable)
        where T : class
    {
        return enumerable.Where(x => x != null)!;
    }

    public static void EnsureDefined<TEnum>(
        this TEnum value, [CallerArgumentExpression(nameof(value))] string? paramName = default)
        where TEnum : struct, Enum
    {
        if (!Enum.IsDefined(value))
        {
            throw new ArgumentException(
                $"The value \"{value}\" is invalid for the \"{typeof(TEnum).Name}\" enum.", paramName);
        }
    }

    public static void EnsureAttributeType(
        this Type type, [CallerArgumentExpression(nameof(type))] string? paramName = default)
    {
        ArgumentNullException.ThrowIfNull(type, paramName);
        if (!typeof(Attribute).IsAssignableFrom(type))
        {
            throw new ArgumentException(
                $"The specified type \"{type.Name}\" is not an attribute type.", paramName);
        }
    }

    public static string GetNameWithoutGenericInfo(this Type type)
    {
        var typeName = type.Name;
        var index = typeName.IndexOf(GenericInfoSeparator, StringComparison.Ordinal);

        return index >= 0 ? typeName[..index] : typeName;
    }
}
