using AutoMapper;
using Ensek.Api.Contracts.Requests;
using Ensek.Api.Contracts.Responses;
using Ensek.Meters.Data.Models;
using Ensek.Meters.Data.Repositories.MeterReadings;
using Ensek.Meters.Domain.Models;
using Ensek.Meters.Domain.Services.Csv;
using Ensek.Meters.Domain.Services.Meters.Caching;
using Ensek.Meters.Domain.Services.Meters.Filtering;
using static Ensek.Meters.Domain.Constants;

namespace Ensek.Meters.Domain.Services.Meters;

public class MeterService : IMeterService
{
    private readonly ICsvReaderService _csvReaderService;
    private readonly IMeterReadingRepository _meterReadingRepository;
    private readonly IAccountMetersRetrievalCachingService _accountRetrievalCachingService;
    private readonly IMeterFilteringService _meterFilteringService;
    private readonly IMapper _mapper;

    public MeterService(
        ICsvReaderService csvReaderService,
        IMeterReadingRepository meterReadingRepository,
        IAccountMetersRetrievalCachingService accountRetrievalCachingService,
        IMeterFilteringService meterFilteringService,
        IMapper mapper)
    {
        _csvReaderService = csvReaderService;
        _meterReadingRepository = meterReadingRepository;
        _accountRetrievalCachingService = accountRetrievalCachingService;
        _meterFilteringService = meterFilteringService;
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
                var accountExists = await _accountRetrievalCachingService.AccountExistsAsync(
                    accountId,
                    cancellationToken);

                if (!accountExists)
                {
                    failedReadings += group.Count();
                    continue;
                }

                var latestMeterReading = await _accountRetrievalCachingService.GetLatestMeterReadingByAccountId(
                    accountId,
                    cancellationToken);

                var validRecords = _meterFilteringService.GetValidRecords(
                    group,
                    latestMeterReading);

                if (!validRecords.Any())
                {
                    failedReadings += group.Count();
                    continue;
                }

                var mappedValidRecords = _mapper.Map<MeterReading[]>(validRecords);
                var latestValidRecord = mappedValidRecords
                    .OrderByDescending(x => x.MeterReadingDateTime)
                    .First();

                _accountRetrievalCachingService.UpdateCache(accountId, latestValidRecord);
                validMeterReadings.AddRange(mappedValidRecords);
                successReadings += mappedValidRecords.Length;
                failedReadings += group.Count() - mappedValidRecords.Length;
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
}
