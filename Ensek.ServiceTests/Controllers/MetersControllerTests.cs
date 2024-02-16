using AutoFixture;
using Ensek.Api;
using Ensek.Api.Contracts.Responses;
using Ensek.Meters.Data;
using Ensek.Meters.Data.Models;
using Ensek.Meters.Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Xunit;

namespace Ensek.ServiceTests.Controllers;

public class MetersControllerTests : IClassFixture<EnsekWebApplicationFactory<Program>>
{
    private const string CsvContentType = "text/csv";
    private const string Endpoint = "/meter-reading-uploads";
    private const string FileName = "file.csv";
    private const string FilePropertyName = "File";

    private readonly EnsekDbContext _dbContext;
    private readonly IFixture _fixture;
    private readonly HttpClient _client;

    public MetersControllerTests(EnsekWebApplicationFactory<Program> factory)
    {
        var scope = factory.Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetService<EnsekDbContext>();
        _fixture = new Fixture();
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Uploading_Valid_And_Invalid_Meter_Readings_Csv_File_Should_Return_Correct_Result()
    {
        // Arrange
        var firstAccountId = 5000;
        var secondAccountId = 5001;
        var validDate = DateTime.UtcNow;
        var validFormatReading = "11111";
        var invalidFormatReading = "11";
        await SeedAccounts([firstAccountId, secondAccountId]);

        var meterReadingCsvAsString = $"AccountId,MeterReadingDateTime,MeterReadValue,\r\n" +
            $"{firstAccountId},{validDate},{validFormatReading},\r\n" +
            $"{secondAccountId},{validDate},{invalidFormatReading},";

        using var streamContent = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(meterReadingCsvAsString)));
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(CsvContentType);
        var formData = new MultipartFormDataContent
        {
            { streamContent, FilePropertyName, FileName }
        };

        // Act
        var response = await _client.PostAsync(Endpoint, formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<MeterReadingsResponse>(
            content, SerializerSettings.Default);

        result.FailedReadings.Should().Be(1);
        result.SuccessReadings.Should().Be(1);
    }

    [Fact]
    public async Task Uploading_Valid_Meter_Readings_Csv_File_With_Non_Existing_Account_Ids_Should_Not_Be_Stored()
    {
        // Arrange
        var firstAccountId = 6000;
        var secondAccountId = 6001;
        var validDate = DateTime.UtcNow;
        var firstValidFormatReading = "11111";
        var secondValidFormatReading = "22222";
        var invalidReadingAcccountId = 6003;
        var invalidReadingSecondAccountId = 6004;
        await SeedAccounts([firstAccountId, secondAccountId]);

        var meterReadingCsvAsString = $"AccountId,MeterReadingDateTime,MeterReadValue,\r\n" +
            $"{invalidReadingAcccountId},{validDate},{firstValidFormatReading},\r\n" +
            $"{invalidReadingSecondAccountId},{validDate},{secondValidFormatReading}";

        using var streamContent = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(meterReadingCsvAsString)));
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(CsvContentType);
        var formData = new MultipartFormDataContent
        {
            { streamContent, FilePropertyName, FileName }
        };

        // Act
        var response = await _client.PostAsync(Endpoint, formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<MeterReadingsResponse>(content, SerializerSettings.Default);

        result.FailedReadings.Should().Be(2);
        result.SuccessReadings.Should().Be(0);
    }

    [Fact]
    public async Task Uploading_Older_Meter_Readings_Than_Existing_Ones_For_The_Same_Existing_Account_Should_Not_Be_Stored()
    {
        // Arrange
        var existingAccountId = 7000;
        var dateTime = DateTime.UtcNow.ToString(Constants.DefaultDateTimeFormat);
        var firstValidFormatReading = "55555";
        var secondValidFormatReading = "44444";
        var thirdValidFormatReading = "33333";
        await SeedAccounts([existingAccountId]);

        var csvAsString = $"AccountId,MeterReadingDateTime,MeterReadValue,\r\n" +
            $"{existingAccountId},{dateTime},{firstValidFormatReading},\r\n" +
            $"{existingAccountId},{dateTime},{secondValidFormatReading},";

        using var streamContent = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(csvAsString)));
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(CsvContentType);
        var formData = new MultipartFormDataContent
        {
            { streamContent, FilePropertyName, FileName }
        };

        await _client.PostAsync(Endpoint, formData);

        // Act
        dateTime = DateTime.MinValue.ToString(Constants.DefaultDateTimeFormat);
        csvAsString = $"AccountId,MeterReadingDateTime,MeterReadValue,\r\n" +
            $"{existingAccountId},{dateTime},{thirdValidFormatReading},";

        using var newStreamContent = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(csvAsString)));
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(CsvContentType);
        formData = new MultipartFormDataContent
        {
            { streamContent, FilePropertyName, FileName }
        };

        var response = await _client.PostAsync(Endpoint, formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<MeterReadingsResponse>(content, SerializerSettings.Default);

        result.FailedReadings.Should().Be(2);
        result.SuccessReadings.Should().Be(0);
    }

    [Fact]
    public async Task Uploading_Valid_Meter_Readings_Csv_File_Should_Not_Store_Same_Reading_Twice()
    {
        // Arrange
        var accountId = 8000;
        var dateTime = DateTime.UtcNow.ToString(Constants.DefaultDateTimeFormat);
        var validFormatReading = "88888";
        await SeedAccounts([accountId]);

        var csvAsString = $"AccountId,MeterReadingDateTime,MeterReadValue,\r\n" +
            $"{accountId},{dateTime},{validFormatReading},\r\n" +
            $"{accountId},{dateTime},{validFormatReading},";

        using var streamContent = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(csvAsString)));
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(CsvContentType);
        var formData = new MultipartFormDataContent
        {
            { streamContent, FilePropertyName, FileName }
        };

        // Act
        var response = await _client.PostAsync(Endpoint, formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<MeterReadingsResponse>(content, SerializerSettings.Default);

        result.FailedReadings.Should().Be(1);
        result.SuccessReadings.Should().Be(1);
    }

    [Fact]
    public async Task Uploading_Invalid_Meter_Readings_Csv_File_Should_Not_Be_Stored()
    {
        // Arrange
        var accountId = 9000;
        var firstDateTime = DateTime.UtcNow.ToString(Constants.DefaultDateTimeFormat);
        var secondDateTime = DateTime.UtcNow.AddDays(1).ToString(Constants.DefaultDateTimeFormat);
        var firstInvalidFormatReading = "1";
        var secondInvalidFormatReading = "2";
        await SeedAccounts([accountId]);

        var csvAsString = $"AccountId,MeterReadingDateTime,MeterReadValue,\r\n" +
            $"{accountId},{firstDateTime},{firstInvalidFormatReading},\r\n" +
            $"{accountId},{secondDateTime},{secondInvalidFormatReading},";

        using var streamContent = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(csvAsString)));
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(CsvContentType);
        var formData = new MultipartFormDataContent
        {
            { streamContent, FilePropertyName, FileName }
        };

        // Act
        var response = await _client.PostAsync(Endpoint, formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<MeterReadingsResponse>(content, SerializerSettings.Default);

        result.FailedReadings.Should().Be(2);
        result.SuccessReadings.Should().Be(0);
    }

    [Fact]
    public async Task Uploading_Meter_Readings_Csv_File_With_Invalid_Headers_Should_Return_BadRequest()
    {
        // Arrange
        var invalidHeader = "AccountId - MAKING THIS HEADER INVALID";
        var validDate = DateTime.UtcNow.ToString(Constants.DefaultDateTimeFormat);
        var validFormatReading = "65444";
        var meterReadingCsvAsString = $"{invalidHeader},MeterReadingDateTime,MeterReadValue,\r\n" +
            $"5,{validDate},{validFormatReading},\r\n" +
            $"6,{validDate},{validFormatReading},";

        using var streamContent = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(meterReadingCsvAsString)));
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(CsvContentType);
        var formData = new MultipartFormDataContent
        {
            { streamContent, FilePropertyName, FileName }
        };

        // Act
        var response = await _client.PostAsync(Endpoint, formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("File contains invalid Headers.");
    }

    [Fact]
    public async Task Uploading_Meter_Readings_Csv_File_With_Invalid_Date_Should_Return_BadRequest()
    {
        // Arrange
        var validDate = DateTime.UtcNow.ToString(Constants.DefaultDateTimeFormat);
        var invalidDate = validDate + "extra characters, making this Date Time invalid.";
        var validFormatReading = "65444";
        var meterReadingCsvAsString = $"AccountId,MeterReadingDateTime,MeterReadValue,\r\n" +
            $"5,{validDate},{validFormatReading},\r\n" +
            $"6,{invalidDate},{validFormatReading},";

        using var streamContent = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(meterReadingCsvAsString)));
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(CsvContentType);
        var formData = new MultipartFormDataContent
        {
            { streamContent, FilePropertyName, FileName }
        };

        // Act
        var response = await _client.PostAsync(Endpoint, formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Unable to read file.");
    }

    [Fact]
    public async Task Uploading_Meter_Readings_Csv_File_With_Invalid_Meter_Read_Value_Should_Return_BadRequest()
    {
        // Arrange
        var validDate = DateTime.UtcNow.ToString(Constants.DefaultDateTimeFormat);
        var validFormatReading = "65444";
        var invalidFormatReading = "This is not a number.";
        var meterReadingCsvAsString = $"AccountId,MeterReadingDateTime,MeterReadValue,\r\n" +
            $"5,{validDate},{validFormatReading},\r\n" +
            $"6,{validDate},{invalidFormatReading},";

        using var streamContent = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(meterReadingCsvAsString)));
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(CsvContentType);
        var formData = new MultipartFormDataContent
        {
            { streamContent, FilePropertyName, FileName }
        };

        // Act
        var response = await _client.PostAsync(Endpoint, formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Unable to read file.");
    }

    private async Task SeedAccounts(long[] ids)
    {
        var accounts = new List<Account>();
        foreach (var id in ids)
        {
            var account = _fixture.Build<Account>()
                .With(x => x.Id, id)
                .Without(x => x.MeterReadings)
                .Create();

            await _dbContext
                .Accounts
                .AddAsync(account);

            await _dbContext.SaveChangesAsync();
        }
    }
}
