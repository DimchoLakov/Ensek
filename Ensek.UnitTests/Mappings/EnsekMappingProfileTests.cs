using AutoFixture;
using AutoMapper;
using Ensek.Meters.Data.Models;
using Ensek.Meters.Domain.MappingProfiles;
using Ensek.Meters.Domain.Models;
using FluentAssertions;
using Xunit;

namespace Ensek.UnitTests.Mappings;

public class EnsekMappingProfileTests
{
    private readonly IFixture _fixture;
    private readonly IMapper _mapper;
    private readonly MapperConfiguration _mapperConfiguration;

    public EnsekMappingProfileTests()
    {
        _fixture = new Fixture();
        _mapperConfiguration = new MapperConfiguration(config => config.AddProfile<EnsekMappingProfile>());
        _mapper = _mapperConfiguration.CreateMapper();
    }

    [Fact]
    public void Mapper_Configuration_Is_Valid()
    {
        _mapperConfiguration.AssertConfigurationIsValid();
    }

    [Fact]
    public void Can_Map_From_MeterReadingCsv_To_MeterReading()
    {
        // Arrange
        var meterReadingCsv = _fixture.Create<MeterReadingCsv>();

        // Act
        var result = _mapper.Map<MeterReading>(meterReadingCsv);

        // Assert
        result.AccountId.Should().Be(meterReadingCsv.AccountId);
        result.MeterReadingDateTime.Should().Be(meterReadingCsv.MeterReadingDateTime);
        result.MeterRead.Should().Be(meterReadingCsv.MeterReadValue);
    }

    [Fact]
    public void Can_Map_From_AccountCsv_To_Account()
    {
        // Arrange
        var accountCsv = _fixture.Create<AccountCsv>();

        // Act
        var result = _mapper.Map<Account>(accountCsv);

        // Assert
        result.Id.Should().Be(accountCsv.AccountId);
        result.FirstName.Should().Be(accountCsv.FirstName);
        result.LastName.Should().Be(accountCsv.LastName);
    }
}
