using Microsoft.AspNetCore.Http;

namespace Ensek.Meters.Domain.Services.Csv;

public interface ICsvReaderService
{
    IEnumerable<T> Read<T>(IFormFile file);

    IEnumerable<T> Read<T>(Stream file);
}
