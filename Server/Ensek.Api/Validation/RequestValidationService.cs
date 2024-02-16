using Ensek.Api.Exceptions;
using FluentValidation;

namespace Ensek.Api.Validation;

public class RequestValidationService : IRequestValidationService
{
    private readonly IServiceProvider _serviceProvider;

    public RequestValidationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task Validate<T>(params T[] requests)
    {
        foreach (var request in requests)
        {
            var validator = _serviceProvider.GetService<IValidator<T>>();
            if (validator != null)
            {
                var vc = new ValidationContext<T>(request);
                var validationResult = await validator.ValidateAsync(vc);
                if (!validationResult.IsValid)
                {
                    throw new ModelValidationException(validationResult.Errors);
                }
            }
        }
    }
}
