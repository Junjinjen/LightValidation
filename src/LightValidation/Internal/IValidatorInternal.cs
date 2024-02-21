using LightValidation.Abstractions;
using LightValidation.Internal.Execute.Validation;

namespace LightValidation.Internal;

internal interface IValidatorInternal<TEntity> : IValidator<TEntity>, IResolverConfiguration
{
    ValidationContext<TEntity> CreateValidationContext(in ValidationContextParameters<TEntity> parameters);

    IValidationExecutor<TEntity> CreateValidationExecutor();
}
