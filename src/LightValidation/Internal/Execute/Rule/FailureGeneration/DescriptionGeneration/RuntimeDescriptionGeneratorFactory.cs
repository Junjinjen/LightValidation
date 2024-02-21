namespace LightValidation.Internal.Execute.Rule.FailureGeneration.DescriptionGeneration;

internal interface IRuntimeDescriptionGeneratorFactory
{
    IErrorDescriptionGenerator<TProperty> Create<TProperty>(in RuntimeDescriptionGeneratorParameters parameters);
}

internal sealed class RuntimeDescriptionGeneratorFactory : IRuntimeDescriptionGeneratorFactory
{
    public IErrorDescriptionGenerator<TProperty> Create<TProperty>(in RuntimeDescriptionGeneratorParameters parameters)
    {
        return new RuntimeDescriptionGenerator<TProperty>(parameters);
    }
}
