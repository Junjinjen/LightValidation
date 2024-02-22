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
    IMetadataProvider CreateMetadataProvider(string key);

    void SetPropertyName(string propertyName);

    void SetErrorCode(string errorCode);

    void SetDefaultErrorCode(string defaultErrorCode);

    void SetErrorDescription(string errorDescription);

    void SetDefaultErrorDescription(string defaultErrorDescription);

    void AddErrorMetadata(string key, object? value);

    void AddErrorMetadata(string key, Func<ValidationContext<TEntity>, TProperty, object?> valueSelector);

    void SetMetadataLocalization(string key, Func<object?, string> localizer);

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
    private readonly IDefaultRuleConfiguration _defaultRuleConfiguration;
    private readonly IMetadataProviderFactory _metadataProviderFactory;

    private readonly Dictionary<string, Func<ValidationContext<TEntity>, TProperty, object?>> _runtimeMetadata = [];
    private readonly Dictionary<string, Func<object?, string>> _metadataLocalizers = [];
    private readonly Dictionary<string, object?> _staticMetadata = [];
    private string? _defaultErrorDescription;
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
        IDefaultRuleConfiguration defaultRuleConfiguration,
        IMetadataProviderFactory metadataProviderFactory)
    {
        _runtimeDescriptionGeneratorFactory = runtimeDescriptionGeneratorFactory;
        _staticDescriptionGeneratorFactory = staticDescriptionGeneratorFactory;
        _runtimeMetadataGeneratorFactory = runtimeMetadataGeneratorFactory;
        _staticMetadataGeneratorFactory = staticMetadataGeneratorFactory;
        _ruleFailureGeneratorFactory = ruleFailureGeneratorFactory;
        _defaultRuleConfiguration = defaultRuleConfiguration;
        _metadataProviderFactory = metadataProviderFactory;
    }

    public IMetadataProvider CreateMetadataProvider(string key)
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

    public void SetErrorCode(string errorCode)
    {
        ArgumentException.ThrowIfNullOrEmpty(errorCode);
        EnsureNotBuilt();

        _errorCode = errorCode;
    }

    public void SetDefaultErrorCode(string defaultErrorCode)
    {
        ArgumentException.ThrowIfNullOrEmpty(defaultErrorCode);
        EnsureNotBuilt();

        _defaultErrorCode = defaultErrorCode;
    }

    public void SetErrorDescription(string errorDescription)
    {
        ArgumentException.ThrowIfNullOrEmpty(errorDescription);
        EnsureNotBuilt();

        _errorDescription = errorDescription;
    }

    public void SetDefaultErrorDescription(string defaultErrorDescription)
    {
        ArgumentException.ThrowIfNullOrEmpty(defaultErrorDescription);
        EnsureNotBuilt();

        _defaultErrorDescription = defaultErrorDescription;
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

    public void SetMetadataLocalization(string key, Func<object?, string> localizer)
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
        var metadataGenerator = CreateMetadataGenerator(insertCollectionIndex);
        var descriptionGenerator = CreateDescriptionGenerator(propertyName, errorDescription, insertCollectionIndex);

        var parameters = new RuleFailureGeneratorParameters<TEntity, TProperty>
        {
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
        if (value == MetadataKey.PropertyName ||
            value == MetadataKey.CollectionIndex ||
            (!allowPropertyValueName && value == MetadataKey.PropertyValue))
        {
            throw new ArgumentException(
                $"Unable to use the reserved metadata key \"{value}\".", paramName);
        }
    }

    private static bool HasPropertyValueFormatting(string errorDescription)
    {
        return errorDescription.ContainsMetadataKey(MetadataKey.PropertyValue);
    }

    private static bool HasCollectionIndexFormatting(string errorDescription)
    {
        return errorDescription.ContainsMetadataKey(MetadataKey.CollectionIndex);
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

    private IErrorMetadataGenerator<TEntity, TProperty> CreateMetadataGenerator(bool insertCollectionIndex)
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
        if (_runtimeMetadata.Count != 0 ||
            insertCollectionIndex ||
            HasPropertyValueFormatting(errorDescription) ||
            HasStaticMetadataForLocalization(errorDescription))
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

        return _runtimeMetadata.Keys.Concat([MetadataKey.CollectionIndex]);
    }
}
