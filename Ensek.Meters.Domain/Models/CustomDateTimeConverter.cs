using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace Ensek.Meters.Domain.Models;

public class CustomDateTimeConverter : DateTimeConverter
{
    public override object ConvertFromString(
        string text,
        IReaderRow row,
        MemberMapData memberMapData)
    {
        return DateTime.Parse(text);
    }
}
