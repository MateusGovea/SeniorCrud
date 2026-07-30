namespace SeniorCrud.Infrastructure.Csv.Options;

public sealed class CsvOptions
{
    public const string SectionName = "Csv";

    public string Delimiter { get; set; } = ",";
}
