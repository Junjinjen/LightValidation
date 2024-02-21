using LightValidation.SourceGenerator.Models;
using Microsoft.CodeAnalysis;
using System.Threading;

namespace LightValidation.SourceGenerator.Internal;

internal interface IMethodInfoCollector
{
    MethodInfo? GetMethodInfo(in GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken);
}

internal sealed class MethodInfoCollector : IMethodInfoCollector
{
    private readonly IConstructorInfoCollector _constructorInfoCollector;
    private readonly IAttributeInfoCollector _attributeInfoCollector;
    private readonly IInterfaceInfoCollector _interfaceInfoCollector;
    private readonly IClassInfoCollector _classInfoCollector;

    public MethodInfoCollector(
        IConstructorInfoCollector constructorInfoCollector,
        IAttributeInfoCollector attributeInfoCollector,
        IInterfaceInfoCollector interfaceInfoCollector,
        IClassInfoCollector classInfoCollector)
    {
        _constructorInfoCollector = constructorInfoCollector;
        _attributeInfoCollector = attributeInfoCollector;
        _interfaceInfoCollector = interfaceInfoCollector;
        _classInfoCollector = classInfoCollector;
    }

    public MethodInfo? GetMethodInfo(in GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
    {
        if (context.Attributes.Length != 1)
        {
            return null;
        }

        var attribute = context.Attributes[0];
        var attributeInfo = _attributeInfoCollector.GetAttributeInfo(attribute, cancellationToken);
        if (attributeInfo == null)
        {
            return null;
        }

        var namedSymbol = (INamedTypeSymbol)context.TargetSymbol;
        var interfaceInfo = _interfaceInfoCollector.GetInterfaceInfo(namedSymbol, cancellationToken);
        if (interfaceInfo == null)
        {
            return null;
        }

        var constructorInfos = _constructorInfoCollector.GetConstructorInfos(namedSymbol, cancellationToken);
        var classInfo = _classInfoCollector.GetClassInfo(namedSymbol, cancellationToken);

        return new MethodInfo(attributeInfo.Value, interfaceInfo.Value, classInfo, constructorInfos);
    }
}
