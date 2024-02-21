using LightValidation.Abstractions.Execute;
using LightValidation.Result;

namespace LightValidation.Internal.Execute.Validation;

internal interface IValidationExecutorFactory
{
    IValidationExecutor<TEntity> Create<TEntity>(
        IEntityValidator<TEntity>[] entityValidators, NullEntityFailure? nullEntityFailure, int metadataCount);
}

internal sealed class ValidationExecutorFactory : IValidationExecutorFactory
{
    public IValidationExecutor<TEntity> Create<TEntity>(
        IEntityValidator<TEntity>[] entityValidators, NullEntityFailure? nullEntityFailure, int metadataCount)
    {
        return new ValidationExecutor<TEntity>(entityValidators, nullEntityFailure)
        {
            MetadataCount = metadataCount,
        };
    }
}
