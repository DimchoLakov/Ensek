using CsvHelper.Configuration.Attributes;

namespace Ensek.Meters.Domain.Models;

public class MeterReadingCsv
{
    [Name("AccountId")]
    public long AccountId { get; set; }

    [TypeConverter(typeof(CustomDateTimeConverter))]
    public DateTime MeterReadingDateTime { get; set; }

    [Name("MeterReadValue")]
    public long MeterReadValue { get; set; }
}
