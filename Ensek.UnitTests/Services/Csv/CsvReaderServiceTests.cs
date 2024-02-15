using Ensek.Meters.Domain.Models;
using Ensek.Meters.Domain.Services.Csv;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Ensek.UnitTests.Services.Csv;

public class CsvReaderServiceTests
{
    [Fact]
    public void Read_Stream_Should_Return_Records()
    {
        // Arrange
        var csvReaderService = new CsvReaderService();
        using var file = File.OpenRead("TestFiles/Meter_Reading.csv");

        // Act
        var result = csvReaderService.Read<MeterReadingCsv>(file).ToArray();

        // Assert
        result.Should().NotBeEmpty();

        result[0].AccountId = 2344;
        result[0].MeterReadingDateTime = "22/04/2019 09:24";
        result[0].AccountId = 1002;

        result[2].AccountId = 8766;
        result[2].MeterReadingDateTime = "22/04/2019 12:25";
        result[2].AccountId = 3440;
    }

    [Fact]
    public void Read_IFormFile_ShouldReturnRecords()
    {
        // Arrange
        var csvReaderService = new CsvReaderService();
        using var file = File.OpenRead("TestFiles/Meter_Reading.csv");
        var formFile = new FormFile(file, 0, file.Length, "test", "test.csv");

        // Act
        var result = csvReaderService.Read<MeterReadingCsv>(formFile).ToArray();

        // Assert
        result.Should().NotBeEmpty();

        result[0].AccountId = 2344;
        result[0].MeterReadingDateTime = "22/04/2019 09:24";
        result[0].AccountId = 1002;

        result[2].AccountId = 8766;
        result[2].MeterReadingDateTime = "22/04/2019 12:25";
        result[2].AccountId = 3440;
    }
}
