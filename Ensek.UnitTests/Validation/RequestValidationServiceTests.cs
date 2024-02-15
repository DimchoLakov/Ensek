namespace Ensek.UnitTests.Validation;

using Ensek.Api.Exceptions;
using Ensek.Api.Validation;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class RequestValidationServiceTests
{
    private readonly IServiceProvider _serviceProvider = Substitute.For<IServiceProvider>();
    private readonly IValidator<SampleRequest> _validator = Substitute.For<IValidator<SampleRequest>>();


    [Fact]
    public async Task Validate_WithValidRequest_NoExceptionThrown()
    {
        // Arrange
        _validator
            .ValidateAsync(Arg.Any<ValidationContext<SampleRequest>>())
            .Returns(new ValidationResult());

        _serviceProvider
            .GetService<IValidator<SampleRequest>>()
            .Returns(_validator);

        var validationService = new RequestValidationService(_serviceProvider);

        // Act & Assert
        var action = () => validationService.Validate(new SampleRequest());

        await action
            .Should()
            .NotThrowAsync();
    }

    [Fact]
    public async Task Validate_WithInvalidRequest_ThrowsModelValidationException()
    {
        // Arrange
        var validationResult = new ValidationResult(
            new List<ValidationFailure>
            {
                new ValidationFailure("Property", "Error message")
            });

        _validator
            .ValidateAsync(Arg.Any<ValidationContext<SampleRequest>>())
            .Returns(validationResult);

        _serviceProvider
            .GetService<IValidator<SampleRequest>>()
            .Returns(_validator);

        var validationService = new RequestValidationService(_serviceProvider);

        // Act & Assert
        var action = () => validationService.Validate(new SampleRequest());

        await action
            .Should()
            .ThrowExactlyAsync<ModelValidationException>();
    }

    public class SampleRequest
    {
        public string Property { get; set; }
    }
}
