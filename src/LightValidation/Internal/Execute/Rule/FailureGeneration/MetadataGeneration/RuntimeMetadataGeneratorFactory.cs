﻿using System;
using System.Collections.Generic;

namespace LightValidation.Internal.Execute.Rule.FailureGeneration.MetadataGeneration;

internal interface IRuntimeMetadataGeneratorFactory
{
    IErrorMetadataGenerator<TEntity, TProperty> Create<TEntity, TProperty>(
        Dictionary<string, Func<ValidationContext<TEntity>, TProperty, object?>> runtimeMetadata,
        Dictionary<string, object?> staticMetadata,
        bool insertCollectionIndex);
}

internal sealed class RuntimeMetadataGeneratorFactory : IRuntimeMetadataGeneratorFactory
{
    public IErrorMetadataGenerator<TEntity, TProperty> Create<TEntity, TProperty>(
        Dictionary<string, Func<ValidationContext<TEntity>, TProperty, object?>> runtimeMetadata,
        Dictionary<string, object?> staticMetadata,
        bool insertCollectionIndex)
    {
        return new RuntimeMetadataGenerator<TEntity, TProperty>(
            runtimeMetadata, staticMetadata, insertCollectionIndex);
    }
}