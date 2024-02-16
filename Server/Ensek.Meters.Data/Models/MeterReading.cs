namespace Ensek.Meters.Data.Models;

public class MeterReading
{
    public long Id { get; set; }
    public long MeterRead { get; set; }
    public DateTime MeterReadingDateTime { get; set; }

    public long AccountId { get; set; }
    public Account Account { get; set; }
}
