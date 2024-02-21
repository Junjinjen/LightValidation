using LightValidation.SourceGenerator.Models;
using Microsoft.CodeAnalysis;
using System.Threading;

namespace LightValidation.SourceGenerator.Internal;

internal interface IInterfaceInfoCollector
{
    InterfaceInfo? GetInterfaceInfo(INamedTypeSymbol symbol, CancellationToken cancellationToken);
}

internal sealed class InterfaceInfoCollector : IInterfaceInfoCollector
{
    private const string RuleBuilderInterfaceFullName = "LightValidation.Abstractions.Build.IRuleBuilder";
    private const string RuleInterfaceFullName = "LightValidation.Abstractions.Build.IRule";
    private const string LightValidationAssemblyName = "LightValidation";
    private const string AddRuleBuilderMethodName = "AddRuleBuilder";
    private const string AddRuleMethodName = "AddRule";
    private const int InterfaceTypeArgumentsCount = 2;
    private const int PropertyTypeIndex = 1;
    private const int EntityTypeIndex = 0;

    private static readonly SymbolDisplayFormat TypeSourceFormat = new(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
        miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes |
            SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

    public InterfaceInfo? GetInterfaceInfo(INamedTypeSymbol symbol, CancellationToken cancellationToken)
    {
        var interfaceSymbol = GetInterfaceSymbol(symbol, out var isRuleBuilder);
        if (interfaceSymbol == null)
        {
            return null;
        }

        if (interfaceSymbol.TypeArguments.Length != InterfaceTypeArgumentsCount)
        {
            return null;
        }

        cancellationToken.ThrowIfCancellationRequested();
        var propertyArgument = interfaceSymbol.TypeArguments[PropertyTypeIndex];
        if (!propertyArgument.IsValid())
        {
            return null;
        }

        var entityArgument = interfaceSymbol.TypeArguments[EntityTypeIndex];
        if (!entityArgument.IsValid())
        {
            return null;
        }

        var propertyTypeSource = propertyArgument.ToDisplayString(TypeSourceFormat);
        var entityTypeSource = entityArgument.ToDisplayString(TypeSourceFormat);
        var methodName = isRuleBuilder ? AddRuleBuilderMethodName : AddRuleMethodName;
        var hasNullablePropertyTypeModifier = propertyArgument.NullableAnnotation == NullableAnnotation.Annotated
            && propertyArgument.IsReferenceType;

        return new InterfaceInfo(hasNullablePropertyTypeModifier, methodName, entityTypeSource, propertyTypeSource);
    }

    private static INamedTypeSymbol? GetInterfaceSymbol(INamedTypeSymbol symbol, out bool isRuleBuilder)
    {
        foreach (var interfaceSymbol in symbol.AllInterfaces)
        {
            if (interfaceSymbol.ContainingAssembly.Name != LightValidationAssemblyName)
            {
                continue;
            }

            var interfaceName = interfaceSymbol.GetFullName();
            if (interfaceName == RuleBuilderInterfaceFullName)
            {
                isRuleBuilder = true;

                return interfaceSymbol;
            }

            if (interfaceName == RuleInterfaceFullName)
            {
                isRuleBuilder = false;

                return interfaceSymbol;
            }
        }

        isRuleBuilder = false;

        return null;
    }
}
