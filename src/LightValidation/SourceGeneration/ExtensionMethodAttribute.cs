using System;

namespace LightValidation.SourceGeneration;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ExtensionMethodAttribute : Attribute
{
    public ExtensionMethodAttribute(string methodName)
    {
        MethodName = methodName;
    }

    public string MethodName { get; }

    public bool IsPublic { get; set; }

    public string? Namespace { get; set; }
}
