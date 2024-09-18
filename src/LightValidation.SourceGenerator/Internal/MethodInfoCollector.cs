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

        var attribute = _attributeInfoCollector.GetAttributeInfo(context.Attributes[0], cancellationToken);
        if (attribute == null)
        {
            return null;
        }

        var namedSymbol = (INamedTypeSymbol)context.TargetSymbol;
        var @interface = _interfaceInfoCollector.GetInterfaceInfo(namedSymbol, cancellationToken);
        if (@interface == null)
        {
            return null;
        }

        var constructors = _constructorInfoCollector.GetConstructorsInfo(namedSymbol, cancellationToken);
        var @class = _classInfoCollector.GetClassInfo(namedSymbol, cancellationToken);

        return new MethodInfo(attribute.Value, @interface.Value, @class, constructors);
    }
}
