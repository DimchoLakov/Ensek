using Ensek.Api.Contracts.Requests;
using FluentValidation;

namespace Ensek.Api.Validation.Validators;

public class UploadMeterReadingsRequestValidator : AbstractValidator<UploadMeterReadingsRequest>
{
    public UploadMeterReadingsRequestValidator()
    {
        const int MaxFileSize = 5 * 1024 * 1024;
        const string AllowedExtension = ".csv";

        RuleFor(x => x.File)
            .NotNull()
            .WithMessage("CSV File is required.");

        When(x => x.File != null, () =>
        {
            RuleFor(x => x.File.FileName)
            .NotNull()
            .NotEmpty()
            .WithMessage("File name cannot be null or empty.")
            .Must(x =>
            {
                if (x == null)
                {
                    return false;
                }

                var extension = Path.GetExtension(x).ToLower();
                return extension == AllowedExtension;
            })
            .WithMessage($"Only {AllowedExtension} files are allowed");

            RuleFor(x => x.File.Length)
            .GreaterThan(0)
            .WithMessage("File must have content.")
            .LessThan(MaxFileSize)
            .WithMessage($"File size cannot be larger than {MaxFileSize}mb.");

            RuleFor(x => x.File.ContentType)
            .NotEmpty()
            .NotNull()
            .WithMessage("Content Type must not be null or empty.")
            .Must(x => x.Contains("text/csv"))
            .WithMessage("Content Type must be \"text/csv\"");

        });
    }
}
