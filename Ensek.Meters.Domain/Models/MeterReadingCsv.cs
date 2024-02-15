namespace Ensek.Meters.Domain.Models;

public class MeterReadingCsv
{
    public long AccountId { get; set; }

    public string MeterReadingDateTime { get; set; }

    public long MeterReadValue { get; set; }
}
