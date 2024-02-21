using Microsoft.CodeAnalysis;
using System.Globalization;

namespace LightValidation.SourceGenerator.Internal;

internal static class UtilityExtensions
{
    private static readonly SymbolDisplayFormat FullNameFormat = new(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);

    public static bool IsValid(this in TypedConstant constant)
    {
        return constant.Kind != TypedConstantKind.Error;
    }

    public static bool IsValid(this ISymbol symbol)
    {
        return symbol.Kind != SymbolKind.ErrorType;
    }

    public static string GetFullName(this INamedTypeSymbol symbol)
    {
        return symbol.ToDisplayString(FullNameFormat);
    }

    public static string FormatIfNotEmpty(this string value, string format)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        return string.Format(CultureInfo.InvariantCulture, format, value);
    }
}
