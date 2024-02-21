using LightValidation.SourceGenerator.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;

namespace LightValidation.SourceGenerator.Internal;

internal interface IAttributeInfoCollector
{
    string AttributeFullName { get; }

    AttributeInfo? GetAttributeInfo(AttributeData attributeData, CancellationToken cancellationToken);
}

internal sealed class AttributeInfoCollector : IAttributeInfoCollector
{
    private const string DefaultNamespace = "LightValidation.Extensions";
    private const string InternalAccessModifierSource = "internal";
    private const string PublicAccessModifierSource = "public";
    private const string NamespaceParameterName = "Namespace";
    private const string IsPublicParameterName = "IsPublic";
    private const int MethodNameConstructorIndex = 0;
    private const bool DefaultIsPublicValue = false;

    public string AttributeFullName => "LightValidation.SourceGeneration.ExtensionMethodAttribute";

    public AttributeInfo? GetAttributeInfo(AttributeData attributeData, CancellationToken cancellationToken)
    {
        var methodName = GetMethodName(attributeData.ConstructorArguments);
        if (string.IsNullOrEmpty(methodName) || !SyntaxFacts.IsValidIdentifier(methodName))
        {
            return null;
        }

        if (!TryParseNamedArguments(attributeData.NamedArguments, out var isPublicIndex, out var namespaceIndex))
        {
            return null;
        }

        cancellationToken.ThrowIfCancellationRequested();
        var isPublic = GetIsPublic(attributeData.NamedArguments, isPublicIndex);
        if (isPublic == null)
        {
            return null;
        }

        var @namespace = GetNamespace(attributeData.NamedArguments, namespaceIndex);
        if (string.IsNullOrEmpty(@namespace))
        {
            return null;
        }

        var accessModifier = isPublic.Value ? PublicAccessModifierSource : InternalAccessModifierSource;

        return new AttributeInfo(accessModifier, methodName!, @namespace!);
    }

    private static string? GetMethodName(in ImmutableArray<TypedConstant> constructorArguments)
    {
        if (constructorArguments.Length <= MethodNameConstructorIndex)
        {
            return null;
        }

        var methodNameArgument = constructorArguments[MethodNameConstructorIndex];
        if (!methodNameArgument.IsValid())
        {
            return null;
        }

        return methodNameArgument.Value as string;
    }

    private static bool TryParseNamedArguments(
        in ImmutableArray<KeyValuePair<string, TypedConstant>> namedArguments,
        out int isPublicIndex,
        out int namespaceIndex)
    {
        isPublicIndex = -1;
        namespaceIndex = -1;

        for (var i = 0; i < namedArguments.Length; i++)
        {
            var key = namedArguments[i].Key;
            if (key == IsPublicParameterName)
            {
                isPublicIndex = i;
                continue;
            }

            if (key == NamespaceParameterName)
            {
                namespaceIndex = i;
                continue;
            }

            return false;
        }

        return true;
    }

    private static bool? GetIsPublic(
        in ImmutableArray<KeyValuePair<string, TypedConstant>> namedArguments, int isPublicIndex)
    {
        if (isPublicIndex < 0)
        {
            return DefaultIsPublicValue;
        }

        var argument = namedArguments[isPublicIndex].Value;
        if (!argument.IsValid())
        {
            return null;
        }

        return argument.Value is bool boolValue ? boolValue : null;
    }

    private static string? GetNamespace(
        in ImmutableArray<KeyValuePair<string, TypedConstant>> namedArguments, int namespaceIndex)
    {
        if (namespaceIndex < 0)
        {
            return DefaultNamespace;
        }

        var argument = namedArguments[namespaceIndex].Value;
        if (!argument.IsValid())
        {
            return null;
        }

        return argument.Value as string;
    }
}
