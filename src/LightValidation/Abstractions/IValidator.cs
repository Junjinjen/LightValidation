using LightValidation.Result;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LightValidation.Abstractions;

public interface IValidator<in TEntity>
{
    ValueTask<ValidationResult> Validate(TEntity? entity, CancellationToken cancellationToken = default);

    ValueTask<ValidationResult> Validate(
        TEntity? entity,
        Action<IValidationOptions>? configurationAction,
        CancellationToken cancellationToken = default);

    ValueTask ValidateAndThrow(TEntity? entity, CancellationToken cancellationToken = default);

    ValueTask ValidateAndThrow(
        TEntity? entity,
        Action<IValidationOptions>? configurationAction,
        CancellationToken cancellationToken = default);
}
