using LightValidation.Abstractions.Execute;

namespace LightValidation.Internal.Build.Rule.FailureGeneration.MetadataProvision;

internal interface IMetadataProviderInternal : IMetadataProvider
{
    object? PopValue();
}

internal sealed class MetadataProvider : IMetadataProviderInternal
{
    private object? _value;

    public void SetValue(object? value)
    {
        _value = value;
    }

    public object? PopValue()
    {
        var result = _value;
        _value = null;

        return result;
    }
}
