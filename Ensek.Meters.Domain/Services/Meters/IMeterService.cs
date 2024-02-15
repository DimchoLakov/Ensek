using Ensek.Api.Contracts.Requests;
using Ensek.Api.Contracts.Responses;

namespace Ensek.Meters.Domain.Services.Meters;

public interface IMeterService
{
    Task<MeterReadingsResponse> ProcessReadings(UploadMeterReadingsRequest uploadMeterReadingsRequest);
}
