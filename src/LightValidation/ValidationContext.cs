using LightValidation.Internal;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LightValidation;

public interface IValidationContext<TError>
{
    IServiceProvider ServiceProvider { get; }

    IReadOnlyList<TError> Errors { get; }

    bool HasErrors { get; }

    IValidationContext<TError> EnsureValid();

    IValidationContext<TError> AddError(TError error);

    IValidator<TModel, TError> Validate<TModel>(TModel model);
}

public class ValidationContext<TError> : IValidationContext<TError>
{
    private readonly List<TError> _errors = [];

    public ValidationContext(IServiceProvider? serviceProvider = null)
    {
        ServiceProvider = serviceProvider ?? new DefaultServiceProvider();
    }

    public IServiceProvider ServiceProvider { get; }

    public IReadOnlyList<TError> Errors => _errors;

    public bool HasErrors => _errors.Count > 0;

    public virtual IValidationContext<TError> EnsureValid()
    {
        if (HasErrors)
        {
            throw new ValidationException<TError>(_errors);
        }

        return this;
    }

    public virtual IValidationContext<TError> AddError(TError error)
    {
        _errors.Add(error);

        return this;
    }

    public virtual IValidator<TModel, TError> Validate<TModel>(TModel model)
    {
        Expression<Func<TModel, TModel>> selectorExpression = x => x;
        var propertyNode = new PropertyNode<TError>();

        return new Validator<TModel, TError>(propertyNode, () => model)
        {
            Context = this,
            PathExpression = selectorExpression,
        };
    }

    private sealed class DefaultServiceProvider : IServiceProvider
    {
        public object? GetService(Type serviceType)
        {
            return null;
        }
    }
}
