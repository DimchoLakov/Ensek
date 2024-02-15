using Ensek.Api;
using Ensek.Api.Contracts.Requests;
using Ensek.Api.Contracts.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Xunit;

namespace Ensek.ServiceTests.Controllers;

public class MetersControllerTests : IClassFixture<EnsekWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly EnsekWebApplicationFactory<Program> _factory;

    public MetersControllerTests(EnsekWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Uploading_Valid_Csv_File_Should_Succeed()
    {
        // Arrange
        using var file = File.OpenRead("TestFiles/Meter_Reading.csv");
        var fileName = "meter-reading.csv";
        var csvContentType = "text/csv";
        var formFile = new FormFile(file, 0, file.Length, file.Name, fileName)
        {
            Headers = new HeaderDictionary
            {
                { "Content-Type", csvContentType }
            }
        };

        var request = new UploadMeterReadingsRequest { File = formFile };
        using var formData = new MultipartFormDataContent { };

        var stringContent = new StringContent(JsonSerializer.Serialize(request, SerializerSettings.Default));
        stringContent.Headers.ContentType = new MediaTypeHeaderValue(csvContentType);

        formData.Add(
            stringContent,
            nameof(request.File),
            request.File.FileName);

        // Act
        var response = await _client.PostAsync("/meter-reading-uploads", formData);

        // Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<MeterReadingsResponse>(content, SerializerSettings.Default);
        result.FailedReadings.Should().Be(3);
        result.SuccessReadings.Should().Be(5);

        //using var scope = _factory.Services.CreateScope();
        //var dbContext = scope.ServiceProvider.GetRequiredService<EnsekDbContext>();
        //var savedEntity = dbContext.MeterReadings.FirstOrDefault();
    }
}
