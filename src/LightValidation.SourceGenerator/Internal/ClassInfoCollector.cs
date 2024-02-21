using LightValidation.SourceGenerator.Models;
using Microsoft.CodeAnalysis;
using System.Threading;

namespace LightValidation.SourceGenerator.Internal;

internal interface IClassInfoCollector
{
    ClassInfo GetClassInfo(INamedTypeSymbol symbol, CancellationToken cancellationToken);
}

internal sealed class ClassInfoCollector : IClassInfoCollector
{
    private static readonly SymbolDisplayFormat ConstraintsFormat = new(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters |
            SymbolDisplayGenericsOptions.IncludeTypeConstraints,
        miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes |
            SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

    private static readonly SymbolDisplayFormat GenericParametersFormat = new(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters);

    public ClassInfo GetClassInfo(INamedTypeSymbol symbol, CancellationToken cancellationToken)
    {
        var withGenericParameters = symbol.ToDisplayString(GenericParametersFormat);
        var withConstraints = symbol.ToDisplayString(ConstraintsFormat);
        var fullName = symbol.GetFullName();

        cancellationToken.ThrowIfCancellationRequested();
        var genericParametersSource = withGenericParameters.Length > fullName.Length
            ? withGenericParameters.Substring(fullName.Length)
            : string.Empty;

        var constraintsSource = withConstraints.Length > withGenericParameters.Length
            ? withConstraints.Substring(withGenericParameters.Length + 1)
            : string.Empty;

        return new ClassInfo(genericParametersSource, constraintsSource, fullName);
    }
}
