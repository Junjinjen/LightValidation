using LightValidation.Result;
using System;

namespace LightValidation.Internal.Build.Validation;

internal interface INullEntityFailureBuilder
{
    void AllowNullEntity(bool value);

    void SetRootPropertyName(string propertyName);

    void SetNullEntityErrorCode(string errorCode);

    void SetNullEntityErrorDescription(string errorDescription);

    NullEntityFailure? Build();
}

internal sealed class NullEntityFailureBuilder : BuilderBase, INullEntityFailureBuilder
{
    private string? _rootPropertyName;
    private string? _errorDescription;
    private string? _errorCode;
    private bool _allowNull;

    public void AllowNullEntity(bool value)
    {
        EnsureNotBuilt();

        _allowNull = value;
    }

    public void SetRootPropertyName(string propertyName)
    {
        ArgumentException.ThrowIfNullOrEmpty(propertyName);
        EnsureNotBuilt();

        _rootPropertyName = propertyName;
    }

    public void SetNullEntityErrorCode(string errorCode)
    {
        ArgumentException.ThrowIfNullOrEmpty(errorCode);
        EnsureNotBuilt();

        _errorCode = errorCode;
    }

    public void SetNullEntityErrorDescription(string errorDescription)
    {
        ArgumentException.ThrowIfNullOrEmpty(errorDescription);
        EnsureNotBuilt();

        _errorDescription = errorDescription;
    }

    public NullEntityFailure? Build()
    {
        SetBuilt();

        if (_allowNull)
        {
            return null;
        }

        var defaultValue = NullEntityFailure.Default;

        if (_rootPropertyName == null && _errorCode == null && _errorDescription == null)
        {
            return defaultValue;
        }

        var propertyName = _rootPropertyName ?? defaultValue.PropertyName;
        var errorCode = _errorCode ?? defaultValue.ErrorCode;
        var errorDescription = GetErrorDescription(defaultValue.ErrorDescription, propertyName);

        return new NullEntityFailure
        {
            PropertyName = propertyName,
            PropertyValue = null,
            ErrorCode = errorCode,
            ErrorDescription = errorDescription,
        };
    }

    private string GetErrorDescription(string defaultDescription, string propertyName)
    {
        if (_errorDescription == null)
        {
            return defaultDescription;
        }

        _errorDescription = _errorDescription.Format(ErrorMetadataKey.PropertyName, propertyName);
        _errorDescription = _errorDescription.Format(ErrorMetadataKey.PropertyValue, null);

        return _errorDescription;
    }
}
