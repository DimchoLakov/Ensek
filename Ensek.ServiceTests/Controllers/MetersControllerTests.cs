using Ensek.Api;
using Ensek.Api.Contracts.Responses;
using FluentAssertions;
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
        var fileContent = File.ReadAllBytes("TestFiles/Meter_Reading.csv");
        using var fileStream = new MemoryStream(fileContent);
        using var streamContent = new StreamContent(fileStream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue("text/csv");

        var formData = new MultipartFormDataContent
        {
            { streamContent, "File", "file.csv" }
        };

        // Act
        var response = await _client.PostAsync("/meter-reading-uploads", formData);

        // Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<MeterReadingsResponse>(content, SerializerSettings.Default);
        result.FailedReadings.Should().Be(0);
        result.SuccessReadings.Should().Be(0);
    }
}
