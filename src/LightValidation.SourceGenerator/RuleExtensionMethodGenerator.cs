using LightValidation.SourceGenerator.Internal;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LightValidation.SourceGenerator;

[Generator]
public sealed class RuleExtensionMethodGenerator : IIncrementalGenerator
{
    private static readonly ExtensionMethodGenerator ExtensionMethodGenerator = new();
    private static readonly ConstructorInfoCollector ConstructorInfoCollector = new();
    private static readonly AttributeInfoCollector AttributeInfoCollector = new();
    private static readonly InterfaceInfoCollector InterfaceInfoCollector = new();
    private static readonly ClassInfoCollector ClassInfoCollector = new();
    private static readonly MethodInfoCollector MethodInfoCollector = new(
        ConstructorInfoCollector, AttributeInfoCollector, InterfaceInfoCollector, ClassInfoCollector);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var methodInfos = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                AttributeInfoCollector.AttributeFullName,
                (node, _) => node is ClassDeclarationSyntax,
                (context, cancellationToken) => MethodInfoCollector.GetMethodInfo(context, cancellationToken))
            .Where(x => x != null);

        context.RegisterSourceOutput(
            methodInfos, (context, info) => ExtensionMethodGenerator.Generate(context, info!.Value));
    }
}
