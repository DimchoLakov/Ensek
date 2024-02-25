namespace Ensek.UnitTests.Services;

using Ensek.Meters.Data.Models;
using Ensek.Meters.Data.Repositories.Accounts;
using Ensek.Meters.Domain.Services.Meters.Caching;
using FluentAssertions;
using NSubstitute;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading;
using Xunit;

public class AccountMetersRetrievalCachingServiceTests
{
    [Fact]
    public async Task AccountExistsAsync_Returns_True_When_Account_Is_Cached()
    {
        // Arrange
        long accountId = 1;
        var cancellationToken = CancellationToken.None;

        var substituteAccountRepository = Substitute.For<IAccountRepository>();
        var cachingService = new AccountMetersRetrievalCachingService(substituteAccountRepository);

        var cacheField = cachingService
            .GetType()
            .GetField(
                "_accountMeterReadingCache",
                BindingFlags.Instance | BindingFlags.NonPublic);

        cacheField.SetValue(cachingService, new ConcurrentDictionary<long, MeterReading>
        {
            [accountId] = new MeterReading()
        });

        // Act
        var result = await cachingService.AccountExistsAsync(accountId, cancellationToken);

        // Assert
        result
            .Should()
            .BeTrue();
    }

    [Fact]
    public async Task AccountExistsAsync_Calls_Repository_When_Account_Is_Not_Cached()
    {
        // Arrange
        long accountId = 1;
        var cancellationToken = CancellationToken.None;

        var accountRepository = Substitute.For<IAccountRepository>();
        accountRepository
            .ExistsAsync(accountId, cancellationToken)
            .Returns(true);

        var cachingService = new AccountMetersRetrievalCachingService(accountRepository);

        // Act
        var result = await cachingService.AccountExistsAsync(accountId, cancellationToken);

        // Assert
        result
            .Should()
            .BeTrue();

        await accountRepository
            .Received(1)
            .ExistsAsync(accountId, cancellationToken);
    }
}
