namespace Ensek.Meters.Data.Models;

public class Account
{
    public long Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public List<MeterReading> MeterReadings { get; set; } = new List<MeterReading>();
}
