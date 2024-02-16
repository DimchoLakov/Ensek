using AutoMapper;
using Ensek.Api.Contracts.Requests;
using Ensek.Api.Contracts.Responses;
using Ensek.Meters.Data.Models;
using Ensek.Meters.Data.Repositories.Accounts;
using Ensek.Meters.Data.Repositories.MeterReadings;
using Ensek.Meters.Domain.Models;
using Ensek.Meters.Domain.Services.Csv;
using static Ensek.Meters.Domain.Constants;

namespace Ensek.Meters.Domain.Services.Meters;

public class MeterService : IMeterService
{
    private readonly ICsvReaderService _csvReaderService;
    private readonly IAccountRepository _accountRepository;
    private readonly IMeterReadingRepository _meterReadingRepository;
    private readonly IMapper _mapper;

    public MeterService(
        ICsvReaderService csvReaderService,
        IAccountRepository accountRepository,
        IMeterReadingRepository meterReadingRepository,
        IMapper mapper)
    {
        _csvReaderService = csvReaderService;
        _accountRepository = accountRepository;
        _meterReadingRepository = meterReadingRepository;
        _mapper = mapper;
    }

    public async Task<MeterReadingsResponse> ProcessReadings(
        UploadMeterReadingsRequest uploadMeterReadingsRequest,
        CancellationToken cancellationToken)
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
                var accountExists = await _accountRepository.ExistsAsync(
                    accountId,
                    cancellationToken);

                if (!accountExists)
                {
                    failedReadings += group.Count();
                    continue;
                }

                var accountWithLatestReading = await _accountRepository.GetAccountByIdWithLatestReadingAsync(
                    accountId,
                    cancellationToken);

                var validRecords = GetValidRecords(
                    group,
                    accountWithLatestReading.LatestMeterReading);

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
                await _meterReadingRepository.BulkSave(
                    validMeterReadings,
                    cancellationToken);
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
        var validRecordsQuery = group.Where(
            x => Math.Abs(x.MeterReadValue).ToString().Length == DefaultMeterReadingValueCount);

        if (latestMeterReadingDateTime != null)
        {
            // Further filter the query to get the readings newer than the latest one
            validRecordsQuery = validRecordsQuery.Where(
                x => latestMeterReadingDateTime.Value < x.MeterReadingDateTime);
        }

        // Make sure we don't get duplicates
        var validRecords = validRecordsQuery
            .DistinctBy(x => new { x.MeterReadValue, x.MeterReadingDateTime })
            .ToList();

        return _mapper.Map<MeterReading[]>(validRecords);
    }
}
