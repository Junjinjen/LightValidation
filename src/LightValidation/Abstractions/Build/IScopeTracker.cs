namespace LightValidation.Abstractions.Build;

public interface IScopeTracker
{
    void EnsureScopeUnchanged(string? errorMessage = null);
}
