using System.Threading.Tasks;

namespace LightValidation.Abstractions.Execute;

public interface IEntityValidator<TEntity>
{
    ExecutionModeCollection ExecutionModes { get; }

    ValueTask Validate(IEntityValidationContext<TEntity> context, ExecutionMode currentMode);
}
