using CsvHelper.Configuration.Attributes;

namespace Ensek.Meters.Domain.Models;

public class AccountCsv
{
    [Name("AccountId")]
    public long AccountId { get; set; }

    [Name("FirstName")]
    public string FirstName { get; set; }

    [Name("LastName")]
    public string LastName { get; set; }
}
