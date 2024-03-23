using LightValidation.SourceGenerator.Models;
using Microsoft.CodeAnalysis;
using System.Text;
using System.Threading;

namespace LightValidation.SourceGenerator.Internal;

internal interface IConstructorInfoCollector
{
    ConstructorInfo[] GetConstructorInfos(INamedTypeSymbol symbol, CancellationToken cancellationToken);
}

internal sealed class ConstructorInfoCollector : IConstructorInfoCollector
{
    private const string RefModifierSource = "ref ";
    private const string OutModifierSource = "out ";
    private const string InModifierSource = "in ";
    private const string ArgumentsSeparator = ", ";
    private const char LeftBracket = '(';
    private const char RightBracket = ')';

    private static readonly SymbolDisplayFormat MethodParametersFormat = new(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
        memberOptions: SymbolDisplayMemberOptions.IncludeParameters | SymbolDisplayMemberOptions.IncludeContainingType,
        parameterOptions: SymbolDisplayParameterOptions.IncludeModifiers |
            SymbolDisplayParameterOptions.IncludeType |
            SymbolDisplayParameterOptions.IncludeName |
            SymbolDisplayParameterOptions.IncludeDefaultValue,
        miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes |
            SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier |
            SymbolDisplayMiscellaneousOptions.AllowDefaultLiteral);

    public ConstructorInfo[] GetConstructorInfos(INamedTypeSymbol symbol, CancellationToken cancellationToken)
    {
        var constructorsLength = symbol.Constructors.Length;
        var result = new ConstructorInfo[constructorsLength];
        for (var i = 0; i < constructorsLength; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            result[i] = GetConstructorInfo(symbol.Constructors[i]);
        }

        return result;
    }

    private static ConstructorInfo GetConstructorInfo(IMethodSymbol symbol)
    {
        if (symbol.Parameters.Length == 0)
        {
            return new ConstructorInfo(parametersSource: string.Empty, argumentsSource: string.Empty);
        }

        var parametersSource = GetParametersSource(symbol);
        var argumentsSource = GetArgumentsSource(symbol);

        return new ConstructorInfo(parametersSource, argumentsSource);
    }

    private static string GetParametersSource(IMethodSymbol symbol)
    {
        var withMethodParameters = symbol.ToDisplayString(MethodParametersFormat);

        var startIndex = withMethodParameters.IndexOf(LeftBracket) + 1;
        var endIndex = withMethodParameters.LastIndexOf(RightBracket);

        return withMethodParameters.Substring(startIndex, endIndex - startIndex);
    }

    private static string GetArgumentsSource(IMethodSymbol symbol)
    {
        var builder = new StringBuilder();

        var parametersCount = symbol.Parameters.Length;
        for (var i = 0; i < parametersCount; i++)
        {
            var parameter = symbol.Parameters[i];
            var modifier = GetRefModifier(parameter.RefKind);

            builder.Append(modifier);
            builder.Append(parameter.Name);

            if (i < parametersCount - 1)
            {
                builder.Append(ArgumentsSeparator);
            }
        }

        return builder.ToString();
    }

    private static string GetRefModifier(RefKind refKind)
    {
        return refKind switch
        {
            RefKind.In or RefKind.RefReadOnlyParameter => InModifierSource,
            RefKind.Out => OutModifierSource,
            RefKind.Ref => RefModifierSource,
            _ => string.Empty,
        };
    }
}
