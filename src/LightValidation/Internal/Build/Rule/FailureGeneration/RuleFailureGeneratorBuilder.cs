using LightValidation.Abstractions.Execute;
using LightValidation.Internal.Build.Rule.FailureGeneration.MetadataProvision;
using LightValidation.Internal.Execute.Rule.FailureGeneration;
using LightValidation.Internal.Execute.Rule.FailureGeneration.DescriptionGeneration;
using LightValidation.Internal.Execute.Rule.FailureGeneration.MetadataGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace LightValidation.Internal.Build.Rule.FailureGeneration;

internal interface IRuleFailureGeneratorBuilder<TEntity, TProperty>
{
    IErrorMetadataProvider CreateErrorMetadataProvider(string key);

    void SetPropertyName(string propertyName);

    void AppendCollectionIndexToPropertyName(bool value, bool isDefaultMode);

    void SetErrorCode(string errorCode, bool isDefaultMode);

    void SetErrorDescription(string errorDescription, bool isDefaultMode);

    void AddErrorMetadata(string key, object? value);

    void AddErrorMetadata(string key, Func<ValidationContext<TEntity>, TProperty, object?> valueSelector);

    void SetErrorMetadataLocalization(string key, Func<object?, string> localizer);

    IRuleFailureGenerator<TEntity, TProperty> Build(string defaultPropertyName, Type ruleType);
}

internal sealed class RuleFailureGeneratorBuilder<TEntity, TProperty>
    : BuilderBase, IRuleFailureGeneratorBuilder<TEntity, TProperty>
{
    private readonly IRuntimeDescriptionGeneratorFactory _runtimeDescriptionGeneratorFactory;
    private readonly IStaticDescriptionGeneratorFactory _staticDescriptionGeneratorFactory;
    private readonly IRuntimeMetadataGeneratorFactory _runtimeMetadataGeneratorFactory;
    private readonly IStaticMetadataGeneratorFactory _staticMetadataGeneratorFactory;
    private readonly IRuleFailureGeneratorFactory _ruleFailureGeneratorFactory;
    private readonly IErrorMetadataProviderFactory _metadataProviderFactory;
    private readonly IDefaultRuleConfiguration _defaultRuleConfiguration;

    private readonly Dictionary<string, Func<ValidationContext<TEntity>, TProperty, object?>> _runtimeMetadata = [];
    private readonly Dictionary<string, Func<object?, string>> _metadataLocalizers = [];
    private readonly Dictionary<string, object?> _staticMetadata = [];
    private bool _defaultAppendCollectionIndex = true;
    private string? _defaultErrorDescription;
    private bool? _appendCollectionIndex;
    private string? _defaultErrorCode;
    private string? _errorDescription;
    private string? _propertyName;
    private string? _errorCode;

    public RuleFailureGeneratorBuilder(
        IRuntimeDescriptionGeneratorFactory runtimeDescriptionGeneratorFactory,
        IStaticDescriptionGeneratorFactory staticDescriptionGeneratorFactory,
        IRuntimeMetadataGeneratorFactory runtimeMetadataGeneratorFactory,
        IStaticMetadataGeneratorFactory staticMetadataGeneratorFactory,
        IRuleFailureGeneratorFactory ruleFailureGeneratorFactory,
        IErrorMetadataProviderFactory metadataProviderFactory,
        IDefaultRuleConfiguration defaultRuleConfiguration)
    {
        _runtimeDescriptionGeneratorFactory = runtimeDescriptionGeneratorFactory;
        _staticDescriptionGeneratorFactory = staticDescriptionGeneratorFactory;
        _runtimeMetadataGeneratorFactory = runtimeMetadataGeneratorFactory;
        _staticMetadataGeneratorFactory = staticMetadataGeneratorFactory;
        _ruleFailureGeneratorFactory = ruleFailureGeneratorFactory;
        _metadataProviderFactory = metadataProviderFactory;
        _defaultRuleConfiguration = defaultRuleConfiguration;
    }

    public IErrorMetadataProvider CreateErrorMetadataProvider(string key)
    {
        ValidateMetadataKey(key, allowPropertyValueName: false);
        EnsureNotBuilt();

        var provider = _metadataProviderFactory.Create();
        _runtimeMetadata.Add(key, (_, _) => provider.PopValue());

        return provider;
    }

    public void SetPropertyName(string propertyName)
    {
        ArgumentException.ThrowIfNullOrEmpty(propertyName);
        EnsureNotBuilt();

        _propertyName = propertyName;
    }

    public void AppendCollectionIndexToPropertyName(bool value, bool isDefaultMode)
    {
        EnsureNotBuilt();

        if (isDefaultMode)
        {
            _defaultAppendCollectionIndex = value;
            return;
        }

        _appendCollectionIndex = value;
    }

    public void SetErrorCode(string errorCode, bool isDefaultMode)
    {
        ArgumentException.ThrowIfNullOrEmpty(errorCode);
        EnsureNotBuilt();

        if (isDefaultMode)
        {
            _defaultErrorCode = errorCode;
            return;
        }

        _errorCode = errorCode;
    }

    public void SetErrorDescription(string errorDescription, bool isDefaultMode)
    {
        ArgumentException.ThrowIfNullOrEmpty(errorDescription);
        EnsureNotBuilt();

        if (isDefaultMode)
        {
            _defaultErrorDescription = errorDescription;
            return;
        }

        _errorDescription = errorDescription;
    }

    public void AddErrorMetadata(string key, object? value)
    {
        ValidateMetadataKey(key, allowPropertyValueName: false);
        EnsureNotBuilt();

        _staticMetadata.Add(key, value);
    }

    public void AddErrorMetadata(string key, Func<ValidationContext<TEntity>, TProperty, object?> valueSelector)
    {
        ValidateMetadataKey(key, allowPropertyValueName: false);
        EnsureNotBuilt();

        _runtimeMetadata.Add(key, valueSelector);
    }

    public void SetErrorMetadataLocalization(string key, Func<object?, string> localizer)
    {
        ValidateMetadataKey(key, allowPropertyValueName: true);
        EnsureNotBuilt();

        _metadataLocalizers[key] = localizer;
    }

    public IRuleFailureGenerator<TEntity, TProperty> Build(string defaultPropertyName, Type ruleType)
    {
        SetBuilt();

        var propertyName = _propertyName ?? defaultPropertyName;

        var errorCode = GetErrorCode(ruleType);
        var errorDescription = GetErrorDescription(ruleType);

        var insertCollectionIndex = HasCollectionIndexFormatting(errorDescription);
        var metadataGenerator = CreateErrorMetadataGenerator(insertCollectionIndex);
        var descriptionGenerator = CreateDescriptionGenerator(propertyName, errorDescription, insertCollectionIndex);

        var parameters = new RuleFailureGeneratorParameters<TEntity, TProperty>
        {
            AppendCollectionIndexToPropertyName = _appendCollectionIndex ?? _defaultAppendCollectionIndex,
            PropertyName = propertyName,
            ErrorCode = errorCode,
            MetadataGenerator = metadataGenerator,
            DescriptionGenerator = descriptionGenerator,
        };

        return _ruleFailureGeneratorFactory.Create(parameters);
    }

    private static void ValidateMetadataKey(
        string value,
        bool allowPropertyValueName,
        [CallerArgumentExpression(nameof(value))] string? paramName = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(value, paramName);
        if (value == ErrorMetadataKey.PropertyName
            || value == ErrorMetadataKey.CollectionIndex
            || (!allowPropertyValueName && value == ErrorMetadataKey.PropertyValue))
        {
            throw new ArgumentException(
                $"Unable to use the reserved metadata key \"{value}\".", paramName);
        }
    }

    private static bool HasPropertyValueFormatting(string errorDescription)
    {
        return errorDescription.ContainsMetadataKey(ErrorMetadataKey.PropertyValue);
    }

    private static bool HasCollectionIndexFormatting(string errorDescription)
    {
        return errorDescription.ContainsMetadataKey(ErrorMetadataKey.CollectionIndex);
    }

    private string GetErrorCode(Type ruleType)
    {
        if (_errorCode != null)
        {
            return _errorCode;
        }

        if (_defaultErrorCode != null)
        {
            return _defaultErrorCode;
        }

        return _defaultRuleConfiguration.GetErrorCode(ruleType);
    }

    private string GetErrorDescription(Type ruleType)
    {
        if (_errorDescription != null)
        {
            return _errorDescription;
        }

        if (_defaultErrorDescription != null)
        {
            return _defaultErrorDescription;
        }

        return _defaultRuleConfiguration.GetErrorDescription(ruleType);
    }

    private IErrorMetadataGenerator<TEntity, TProperty> CreateErrorMetadataGenerator(bool insertCollectionIndex)
    {
        if (_runtimeMetadata.Count != 0 || insertCollectionIndex)
        {
            return _runtimeMetadataGeneratorFactory.Create(_runtimeMetadata, _staticMetadata, insertCollectionIndex);
        }

        return _staticMetadataGeneratorFactory.Create<TEntity, TProperty>(_staticMetadata);
    }

    private IErrorDescriptionGenerator<TProperty> CreateDescriptionGenerator(
        string propertyName, string errorDescription, bool insertCollectionIndex)
    {
        if (_runtimeMetadata.Count != 0
            || insertCollectionIndex
            || HasPropertyValueFormatting(errorDescription)
            || HasStaticMetadataForLocalization(errorDescription))
        {
            var runtimeMetadataKeys = GetRuntimeMetadataKeys(insertCollectionIndex);

            var parameters = new RuntimeDescriptionGeneratorParameters
            {
                PropertyName = propertyName,
                ErrorDescription = errorDescription,
                StaticMetadata = _staticMetadata,
                RuntimeMetadataKeys = runtimeMetadataKeys,
                MetadataLocalizers = _metadataLocalizers,
            };

            return _runtimeDescriptionGeneratorFactory.Create<TProperty>(parameters);
        }

        return _staticDescriptionGeneratorFactory.Create<TProperty>(propertyName, errorDescription, _staticMetadata);
    }

    private bool HasStaticMetadataForLocalization(string errorDescription)
    {
        return _staticMetadata.Any(
            x => errorDescription.ContainsMetadataKey(x.Key) && _metadataLocalizers.ContainsKey(x.Key));
    }

    private IEnumerable<string> GetRuntimeMetadataKeys(bool insertCollectionIndex)
    {
        if (!insertCollectionIndex)
        {
            return _runtimeMetadata.Keys;
        }

        return _runtimeMetadata.Keys.Concat([ErrorMetadataKey.CollectionIndex]);
    }
}
