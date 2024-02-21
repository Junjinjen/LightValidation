namespace LightValidation.Abstractions;

public interface IDependencyResolver
{
    TService GetService<TService>();
}
