using System.Threading.Tasks;

namespace LightValidation.Abstractions.Execute;

public interface IPropertyRule<TEntity, in TProperty>
{
    ValueTask<bool> Validate(ValidationContext<TEntity> context, TProperty value);
}
