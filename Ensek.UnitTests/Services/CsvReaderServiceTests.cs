using Ensek.Meters.Domain.Services.Csv;
using FluentAssertions;
using System.Text;
using Xunit;

namespace Ensek.UnitTests.Services;

public class CsvReaderServiceTests
{
    private readonly ICsvReaderService _csvReaderService;

    public CsvReaderServiceTests()
    {
        _csvReaderService = new CsvReaderService();
    }

    [Fact]
    public async Task ReadCsvFileInBatches_Should_Return_Empty_List_When_Stream_Is_Empty()
    {
        // Arrange
        using var ms = new MemoryStream();

        // Act
        var asyncEnumerable = _csvReaderService.ReadCsvFileInBatches<TestClass>(ms, batchSize: 10);

        // Assert
        var result = await GetResult(asyncEnumerable);
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ReadCsvFileInBatches_Should_Return_All_Records()
    {
        // Arrange
        var valueOne = "Value1";
        var valueTwo = "Value2";
        var valueThree = "Value3";
        var valueFour = "Value4";
        var csvData = $"{nameof(TestClass.HeaderOne)},{nameof(TestClass.HeaderTwo)}," +
            $"\n\r{valueOne},{valueTwo}," +
            $"\n\r{valueThree},{valueFour}," +
            $"\n\rValue5,Value55," +
            $"\n\rValue6,Value66," +
            $"\n\rValue7,Value77," +
            $"\n\rValue8,Value88," +
            $"\n\rValue9,Value99," +
            $"\n\rValue10,Value1010,";

        using var ms = new MemoryStream(Encoding.UTF8.GetBytes(csvData));

        // Act
        var asyncEnumerable = _csvReaderService.ReadCsvFileInBatches<TestClass>(ms, batchSize: 3);
        var result = await GetResult(asyncEnumerable);

        result.Count.Should().Be(8);
        result[0].HeaderOne.Should().Be(valueOne);
        result[0].HeaderTwo.Should().Be(valueTwo);
        result[1].HeaderOne.Should().Be(valueThree);
        result[1].HeaderTwo.Should().Be(valueFour);
    }

    private static async Task<List<TestClass>> GetResult(IAsyncEnumerable<List<TestClass>> asyncEnumerable)
    {

        // Assert
        var result = new List<TestClass>();
        await foreach (var list in asyncEnumerable)
        {
            foreach (var listItem in list)
            {
                result.Add(listItem);
            }
        }

        return result;
    }

    // Add more test cases as needed to cover different scenarios

    private class TestClass
    {
        public string HeaderOne { get; set; }

        public string HeaderTwo { get; set; }
    }
}
