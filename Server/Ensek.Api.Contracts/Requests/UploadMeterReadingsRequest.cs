using Microsoft.AspNetCore.Http;

namespace Ensek.Api.Contracts.Requests;

public class UploadMeterReadingsRequest
{
    public IFormFile File { get; set; }
}
