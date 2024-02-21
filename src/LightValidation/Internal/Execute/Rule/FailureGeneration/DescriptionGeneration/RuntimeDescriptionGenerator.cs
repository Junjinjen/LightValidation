using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace LightValidation.Internal.Execute.Rule.FailureGeneration.DescriptionGeneration;

internal sealed class RuntimeDescriptionGenerator<TProperty> : IErrorDescriptionGenerator<TProperty>
{
    private const int EscapedBracesCount = 2;
    private const int KeyBracesCount = 2;
    private const char LeftBrace = '{';
    private const char RightBrace = '}';

    private readonly KeyValuePair<string, Func<object?, string>?>[] _metadataFormatters;
    private readonly CompositeFormat _compositeFormat;

    public RuntimeDescriptionGenerator(in RuntimeDescriptionGeneratorParameters parameters)
    {
        var description = PreFormatDescription(
            parameters.ErrorDescription,
            parameters.PropertyName,
            parameters.StaticMetadata,
            parameters.MetadataLocalizers);

        var allKeys = parameters.StaticMetadata.Keys
            .Concat(parameters.RuntimeMetadataKeys).Concat([MetadataKey.PropertyValue]);

        var localizers = parameters.MetadataLocalizers;
        _metadataFormatters = allKeys
            .Select(x => new { Index = description.GetMetadataKeyIndex(x), Key = x })
            .Where(x => x.Index >= 0)
            .OrderBy(x => x.Index)
            .Select(x => KeyValuePair.Create(x.Key, localizers.GetValueOrDefault(x.Key)))
            .ToArray();

        Debug.Assert(_metadataFormatters.Length != 0, "Runtime description generator must have dynamic formatting.");

        description = EscapeDescription(description);

        for (var i = 0; i < _metadataFormatters.Length; i++)
        {
            var key = _metadataFormatters[i].Key;
            description = description.Format(key, $"{{{i}}}");
        }

        _compositeFormat = CompositeFormat.Parse(description);
    }

    public string Generate(TProperty propertyValue, IReadOnlyDictionary<string, object?> errorMetadata)
    {
        var formattersCount = _metadataFormatters.Length;
        var arguments = ArrayPool<object?>.Shared.Rent(formattersCount);

        try
        {
            for (var i = 0; i < formattersCount; i++)
            {
                var key = _metadataFormatters[i].Key;
                var localizer = _metadataFormatters[i].Value;
                var metadata = key != MetadataKey.PropertyValue ? errorMetadata[key] : propertyValue;

                arguments[i] = localizer == null ? metadata : localizer.Invoke(metadata);
            }

            return string.Format(CultureInfo.InvariantCulture, _compositeFormat, arguments);
        }
        finally
        {
            Array.Clear(arguments, 0, formattersCount);
            ArrayPool<object?>.Shared.Return(arguments);
        }
    }

    private static string PreFormatDescription(
        string description,
        string propertyName,
        Dictionary<string, object?> staticMetadata,
        Dictionary<string, Func<object?, string>> localizers)
    {
        description = description.Format(MetadataKey.PropertyName, propertyName);

        var metadataWithoutLocalizers = staticMetadata.Where(x => !localizers.ContainsKey(x.Key));
        foreach (var metadata in metadataWithoutLocalizers)
        {
            description = description.Format(metadata.Key, metadata.Value);
        }

        return description;
    }

    private string EscapeDescription(string description)
    {
        var builder = new StringBuilder(description.Length);

        var keysMap = _metadataFormatters
            .Select(x => KeyValuePair.Create(description.GetMetadataKeyIndex(x.Key), x.Key.Length + KeyBracesCount))
            .Where(x => x.Key >= 0)
            .ToDictionary(x => x.Key, x => x.Value);

        var index = 0;
        while (index < description.Length)
        {
            if (description[index] == LeftBrace)
            {
                if (keysMap.TryGetValue(index, out var skipLength))
                {
                    builder.Append(description.AsSpan(index, skipLength));
                    index += skipLength;

                    continue;
                }

                builder.Append(LeftBrace, EscapedBracesCount);
                index++;

                continue;
            }

            if (description[index] == RightBrace)
            {
                builder.Append(RightBrace, EscapedBracesCount);
                index++;

                continue;
            }

            builder.Append(description[index]);
            index++;
        }

        return builder.ToString();
    }
}
