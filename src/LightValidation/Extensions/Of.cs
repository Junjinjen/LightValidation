using System.Diagnostics.CodeAnalysis;

namespace LightValidation.Extensions;

[SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Fluent syntax")]
public static class Of
{
    public static Of<T>? Type<T>()
    {
        return null;
    }
}

[SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Fluent syntax")]
[SuppressMessage("Major Code Smell", "S2326:Unused type parameters should be removed", Justification = "Type holder")]
public sealed class Of<T>
{
}
