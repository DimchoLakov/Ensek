namespace Ensek.UnitTests.Validation;

using Ensek.Api.Contracts.Requests;
using Ensek.Api.Validation.Validators;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using System.IO;
using Xunit;

public class UploadMeterReadingsRequestValidatorTests
{
    const string ValidFileName = "validFileName.csv";
    const string AnotherValidFileName = "anotherValidFileName.CSV";
    const string InvalidFileName = "invalidFile.txt";

    private readonly UploadMeterReadingsRequestValidator _validator;

    public UploadMeterReadingsRequestValidatorTests()
    {
        _validator = new UploadMeterReadingsRequestValidator();
    }

    [Fact]
    public void Validate_When_File_Is_Null_Should_Have_Validation_Error_For_File()
    {
        // Arrange
        var request = new UploadMeterReadingsRequest { File = null };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.File);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_When_File_Name_Is_Null_Or_Empty_Should_Have_Validation_Error_For_FileName(string fileName)
    {
        // Arrange
        var file = new FormFile(Stream.Null, 0, 100, fileName, fileName)
        {
            Headers = new HeaderDictionary { { "Content-Type", "text/csv" } }
        };
        var request = new UploadMeterReadingsRequest { File = file };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.File.FileName);
    }

    [Fact]
    public void Validate_When_File_Extension_Is_Invalid_Should_Have_Validation_Error_For_File_Name()
    {
        // Arrange
        var file = new FormFile(Stream.Null, 0, 100, InvalidFileName, InvalidFileName)
        {
            Headers = new HeaderDictionary { { "Content-Type", "text/csv" } }
        };
        var request = new UploadMeterReadingsRequest { File = file };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.File.FileName);
    }

    [Theory]
    [InlineData(ValidFileName)]
    [InlineData(AnotherValidFileName)]
    public void Validate_When_FileName_And_Extension_Are_Valid_Should_Not_Have_Validation_Errors(string fileName)
    {
        // Arrange
        var file = new FormFile(Stream.Null, 0, 100, fileName, fileName)
        {
            Headers = new HeaderDictionary { { "Content-Type", "text/csv" } }
        };
        var request = new UploadMeterReadingsRequest { File = file };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.File.FileName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_When_File_Content_Type_Is_Null_Or_Empty_Should_Have_Validation_Error_For_File_Content_Type(string contentType)
    {
        // Arrange
        var file = new FormFile(Stream.Null, 0, 100, ValidFileName, ValidFileName)
        {
            Headers = new HeaderDictionary { { "Content-Type", contentType } }
        };
        var request = new UploadMeterReadingsRequest { File = file };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.File.ContentType);
    }

    [Fact]
    public void Validate_When_File_Content_Type_Is_Invalid_Should_Have_Validation_Error_For_File_Content_Type()
    {
        // Arrange
        var file = new FormFile(Stream.Null, 0, 100, ValidFileName, ValidFileName)
        {
            Headers = new HeaderDictionary { { "Content-Type", "application/json" } }
        };
        var request = new UploadMeterReadingsRequest { File = file };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.File.ContentType);
    }

    [Fact]
    public void Validate_When_File_Content_Type_Is_Valid_Should_Not_Have_Validation_Errors()
    {
        // Arrange
        var file = new FormFile(Stream.Null, 0, 100, ValidFileName, ValidFileName)
        {
            Headers = new HeaderDictionary { { "Content-Type", "text/csv" } }
        };
        var request = new UploadMeterReadingsRequest { File = file };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.File.ContentType);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10 * 1024 * 1024)] // Exceeds the maximum allowed file size
    public void Validate_When_File_Length_Is_Invalid_Should_Have_Validation_Error_For_File_Length(int fileLength)
    {
        // Arrange
        var file = new FormFile(Stream.Null, 0, fileLength, ValidFileName, ValidFileName)
        {
            Headers = new HeaderDictionary { { "Content-Type", "text/csv" } }
        };
        var request = new UploadMeterReadingsRequest { File = file };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.File.Length);
    }

    [Fact]
    public void Validate_When_File_Length_Is_Valid_Should_Not_Have_Validation_Errors()
    {
        // Arrange
        var file = new FormFile(Stream.Null, 0, 1024, ValidFileName, ValidFileName)
        {
            Headers = new HeaderDictionary { { "Content-Type", "text/csv" } }
        };
        var request = new UploadMeterReadingsRequest { File = file };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.File.Length);
    }

}

