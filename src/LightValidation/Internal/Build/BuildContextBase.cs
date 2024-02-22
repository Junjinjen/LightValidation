using System;
using System.Diagnostics;

namespace LightValidation.Internal.Build;

internal abstract class BuildContextBase
{
    private const string DefaultErrorMessage = "Unable to access build context data after validation has been built.";

    private readonly string _errorMessage;

    protected BuildContextBase(string errorMessage)
    {
        _errorMessage = errorMessage;
    }

    protected BuildContextBase()
        : this(DefaultErrorMessage)
    {
    }

    protected abstract bool IsBuilt { get; }

    [DebuggerStepThrough]
    protected void EnsureNotBuilt()
    {
        if (IsBuilt)
        {
            throw new InvalidOperationException(_errorMessage);
        }
    }

    [DebuggerStepThrough]
    protected T ReturnWithBuildCheck<T>(T value)
    {
        EnsureNotBuilt();

        return value;
    }
}
