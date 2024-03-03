using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace Ensek.Meters.Domain.Services.Csv;

public class CsvReaderService : ICsvReaderService
{
    public async IAsyncEnumerable<List<T>> ReadCsvFileInBatches<T>(Stream stream, int batchSize)
    {
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));
        var batch = new List<T>();

        while (await csv.ReadAsync())
        {
            var record = csv.GetRecord<T>();
            batch.Add(record);

            // if records read reaches batch size return the batch, and reset batch list
            if (batch.Count % batchSize == 0)
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
