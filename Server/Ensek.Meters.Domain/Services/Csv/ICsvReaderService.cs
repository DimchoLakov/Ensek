using Microsoft.AspNetCore.Http;

namespace Ensek.Meters.Domain.Services.Csv;

public interface ICsvReaderService
{
    IAsyncEnumerable<List<T>> ReadCsvFileInBatches<T>(Stream stream, int batchSize);
}
