using AutoMapper;
using Ensek.Api.Contracts.Requests;
using Ensek.Api.Contracts.Responses;
using Ensek.Meters.Data;
using Ensek.Meters.Data.Models;
using Ensek.Meters.Domain.Models;
using Ensek.Meters.Domain.Services.Csv;
using Microsoft.EntityFrameworkCore;
using static Ensek.Meters.Domain.Constants;

namespace Ensek.Meters.Domain.Services.Meters;

public class MeterService : IMeterService
{
    private readonly ICsvReaderService _csvReaderService;
    private readonly EnsekDbContext _ensekDbContext;
    private readonly IMapper _mapper;

    public MeterService(
        ICsvReaderService csvReaderService,
        EnsekDbContext ensekDbContext,
        IMapper mapper)
    {
        _csvReaderService = csvReaderService;
        _ensekDbContext = ensekDbContext;
        _mapper = mapper;
    }

    public async Task<MeterReadingsResponse> ProcessReadings(UploadMeterReadingsRequest uploadMeterReadingsRequest)
    {
        var batches = _csvReaderService.ReadCsvFileInBatches<MeterReadingCsv>(
            uploadMeterReadingsRequest.File.OpenReadStream(),
            DefaultBatchSize);

        var failedReadings = 0;
        var successReadings = 0;

        await foreach (var batch in batches)
        {
            var readingsGroupedByAccountId = batch
                .GroupBy(x => x.AccountId)
                .ToArray();

            var validMeterReadings = new List<MeterReading>();

            foreach (var group in readingsGroupedByAccountId)
            {
                var accountId = group.Key;
                var account = await _ensekDbContext.Accounts.FindAsync(accountId);
                if (account == null)
                {
                    continue;
                }

                var accountWithLatestReading = await _ensekDbContext
                    .Accounts
                    .Include(x => x.MeterReadings)
                    .Where(x => x.Id == accountId)
                    .Select(x => new
                    {
                        AccountId = x.Id,
                        LatestMeterReading = x.MeterReadings
                            .OrderByDescending(m => m.MeterReadingDateTime)
                            .FirstOrDefault()
                    })
                    .FirstOrDefaultAsync();

                var validRecords = GetValidRecords(group, accountWithLatestReading.LatestMeterReading);
                if (!validRecords.Any())
                {
                    failedReadings += group.Count();
                    continue;
                }

                validMeterReadings.AddRange(validRecords);
                successReadings += validRecords.Length;
                failedReadings += group.Count() - validRecords.Length;
            }

            if (validMeterReadings.Any())
            {
                await _ensekDbContext.MeterReadings.AddRangeAsync(validMeterReadings);
                await _ensekDbContext.SaveChangesAsync();
            }
        }

        return new MeterReadingsResponse
        {
            FailedReadings = failedReadings,
            SuccessReadings = successReadings
        };
    }

    private MeterReading[] GetValidRecords(
        IGrouping<long, MeterReadingCsv> group,
        MeterReading latestMeterReading)
    {
        var latestMeterReadingDateTime = latestMeterReading?.MeterReadingDateTime;

        // Get readings with values which have 5 digits in total
        var validRecordsQuery = group.Where(x => Math.Abs(x.MeterReadValue).ToString().Length == DefaultMeterReadingValueCount);

        if (latestMeterReadingDateTime != null)
        {
            // Further filter the query to get the readings newer than the latest one
            validRecordsQuery = validRecordsQuery.Where(x => latestMeterReadingDateTime.Value < x.MeterReadingDateTime);
        }

        // Make sure we don't get duplicates
        var validRecords = validRecordsQuery
            .DistinctBy(x => new { x.MeterReadValue, x.MeterReadingDateTime })
            .ToList();

        return _mapper.Map<MeterReading[]>(validRecords);
    }
}
