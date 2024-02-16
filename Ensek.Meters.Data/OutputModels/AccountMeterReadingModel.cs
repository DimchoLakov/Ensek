using Ensek.Meters.Data.Models;

namespace Ensek.Meters.Data.OutputModels;

public class AccountMeterReadingModel
{
    public long AccountId { get; set; }

    public MeterReading LatestMeterReading { get; set; }
}
