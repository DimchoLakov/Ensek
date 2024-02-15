
using CsvHelper;
using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace Ensek.Meters.Domain.Services.Csv;

public class CsvReaderService : ICsvReaderService
{
    public IEnumerable<T> Read<T>(Stream stream)
    {
        using var reader = new StreamReader(stream);

        return ReadFile<T>(reader);
    }

    public IEnumerable<T> Read<T>(IFormFile file)
    {
        using var reader = new StreamReader(file.OpenReadStream());

        return ReadFile<T>(reader);
    }

    private static IEnumerable<T> ReadFile<T>(StreamReader streamReader)
    {
        using var csv = new CsvReader(streamReader, CultureInfo.InvariantCulture);
        var records = csv.GetRecords<T>().ToArray();

        return records;
    }
}
