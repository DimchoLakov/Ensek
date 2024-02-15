using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using Ensek.Api.Contracts.Requests;
using Ensek.Api.Contracts.Responses;
using Ensek.Meters.Data;
using Ensek.Meters.Data.Models;
using Ensek.Meters.Domain.Models;
using Ensek.Meters.Domain.Services.Csv;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

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
        var batches = ReadCsvFileInBatches<MeterReadingCsv>(uploadMeterReadingsRequest.File.OpenReadStream(), 1000);
        var failedReadings = 0;
        var successReadings = 0;

        await foreach (var batch in batches)
        {
            var readingsGroupedByAccountId = batch
                .GroupBy(x => x.AccountId)
                .ToArray();

            foreach (var group in readingsGroupedByAccountId)
            {
                var accountId = group.Key;

                // Ignore readings with no assciated account
                var account = await _ensekDbContext
                    .Accounts
                    .FindAsync(accountId);

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

                var latestMeterReadingDateTime = accountWithLatestReading
                    .LatestMeterReading
                    ?.MeterReadingDateTime;

                var validRecordsQuery = group.Where(x => x.MeterReadValue.ToString().Length == 5);
                if (latestMeterReadingDateTime != null)
                {
                    validRecordsQuery = validRecordsQuery
                        .Where(x => latestMeterReadingDateTime.Value < DateTime.ParseExact(
                            x.MeterReadingDateTime, "dd/MM/yyyy hh:mm", CultureInfo.InvariantCulture));
                }

                var validRecords = validRecordsQuery
                    .DistinctBy(x => new { x.MeterReadValue, x.MeterReadingDateTime })
                    .ToArray();

                if (!validRecords.Any())
                {
                    continue;
                }

                var recordsToAdd = _mapper.Map<MeterReading[]>(validRecords);

                await _ensekDbContext
                    .MeterReadings
                    .AddRangeAsync(recordsToAdd);

                successReadings += recordsToAdd.Length;
                failedReadings += group.Count() - recordsToAdd.Length;
            }
        }

        if (successReadings != 0)
        {
            await _ensekDbContext.SaveChangesAsync();
        }

        // return success readings and failed readings

        return new MeterReadingsResponse
        {
            FailedReadings = failedReadings,
            SuccessReadings = successReadings
        };
    }

    private static async IAsyncEnumerable<List<T>> ReadCsvFileInBatches<T>(Stream stream, int batchSize)
    {
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));
        var batch = new List<T>();
        int recordsRead = 0;

        var header = csv.HeaderRecord;

        while (await csv.ReadAsync())
        {
            var record = csv.GetRecord<T>();
            batch.Add(record);
            recordsRead++;

            if (recordsRead % batchSize == 0)
            {
                yield return batch;
                batch = new List<T>();
            }
        }

        // Return the last batch (if any)
        if (batch.Count > 0)
        {
            yield return batch;
        }
    }
}
