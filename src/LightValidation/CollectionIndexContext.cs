using System.Text;

namespace LightValidation;

public readonly struct CollectionIndexContext<TEntity, TProperty>
{
    public required StringBuilder StringBuilder { get; init; }

    public required TEntity Entity { get; init; }

    public required TProperty Element { get; init; }

    public required int Index { get; init; }
}
