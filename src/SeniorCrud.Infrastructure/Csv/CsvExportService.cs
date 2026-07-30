using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Options;
using SeniorCrud.Application.Abstractions.Csv;
using SeniorCrud.Infrastructure.Csv.Options;

namespace SeniorCrud.Infrastructure.Csv;

public sealed class CsvExportService : ICsvExportService
{
    private readonly CsvOptions _options;

    public CsvExportService(IOptions<CsvOptions> options)
    {
        _options = options.Value;
    }

    public string Export<T>(IEnumerable<T> records)
    {
        var writer = new StringWriter();
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = _options.Delimiter
        };

        using var csv = new CsvWriter(writer, config);
        csv.WriteRecords(records ?? []);
        writer.Flush();

        return writer.ToString();
    }
}
