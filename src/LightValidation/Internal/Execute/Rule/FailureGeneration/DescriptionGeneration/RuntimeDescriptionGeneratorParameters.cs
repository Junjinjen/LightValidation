using System;
using System.Collections.Generic;

namespace LightValidation.Internal.Execute.Rule.FailureGeneration.DescriptionGeneration;

internal readonly ref struct RuntimeDescriptionGeneratorParameters
{
    public required string PropertyName { get; init; }

    public required string ErrorDescription { get; init; }

    public required Dictionary<string, object?> StaticMetadata { get; init; }

    public required IEnumerable<string> RuntimeMetadataKeys { get; init; }

    public required Dictionary<string, Func<object?, string>> MetadataLocalizers { get; init; }
}
