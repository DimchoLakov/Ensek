using Ensek.Api.Contracts.Requests;
using Ensek.Api.Contracts.Responses;

namespace Ensek.Meters.Domain.Services;

public interface IMeterService
{
    Task<MeterReadingsResponse> SaveReadings(UploadMeterReadingsRequest uploadMeterReadingsRequest);
}
