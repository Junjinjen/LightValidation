namespace LightValidation.Internal.Build.Validation.Context;

internal interface IEntityContextFactory
{
    IEntityContextInternal Create(in EntityContextParameters parameters);
}

internal sealed class EntityContextFactory : IEntityContextFactory
{
    public IEntityContextInternal Create(in EntityContextParameters parameters)
    {
        return new EntityContext(
            parameters.DefaultExecutionMode,
            parameters.ValidatorType,
            parameters.ExecutionModeByAttribute,
            parameters.PropertyNames);
    }
}
