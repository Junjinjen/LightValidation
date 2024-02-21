using System;
using System.Diagnostics;

namespace LightValidation.Internal.Build;

internal abstract class BuilderBase
{
    private const string DefaultErrorMessage = "Unable to perform an operation after validation has been built.";

    private readonly string _errorMessage;

    private bool _built;

    protected BuilderBase(string errorMessage)
    {
        _errorMessage = errorMessage;
    }

    protected BuilderBase()
        : this(DefaultErrorMessage)
    {
    }

    [DebuggerStepThrough]
    protected void EnsureNotBuilt()
    {
        if (_built)
        {
            throw new InvalidOperationException(_errorMessage);
        }
    }

    [DebuggerStepThrough]
    protected void SetBuilt()
    {
        EnsureNotBuilt();

        _built = true;
    }
}
