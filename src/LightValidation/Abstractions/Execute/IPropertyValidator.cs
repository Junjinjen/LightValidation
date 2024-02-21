using System.Threading.Tasks;

namespace LightValidation.Abstractions.Execute;

public interface IPropertyValidator<TEntity, in TProperty>
{
    ExecutionModeCollection ExecutionModes { get; }

    ValueTask Validate(IPropertyValidationContext<TEntity, TProperty> context, ExecutionMode currentMode);
}
