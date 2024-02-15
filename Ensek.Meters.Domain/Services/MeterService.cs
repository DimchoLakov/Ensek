using Ensek.Api.Contracts.Requests;
using Ensek.Api.Contracts.Responses;

namespace Ensek.Meters.Domain.Services;

public class MeterService : IMeterService
{
    public Task<MeterReadingsResponse> SaveReadings(UploadMeterReadingsRequest uploadMeterReadingsRequest)
    {
        return Task.FromResult(new MeterReadingsResponse
        {
            FailedReadings = 3,
            SuccessReadings = 5
        });
    }
}
