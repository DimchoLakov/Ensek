namespace Ensek.Api.Validation;

public interface IRequestValidationService
{
    Task Validate<T>(params T[] requests);
}
