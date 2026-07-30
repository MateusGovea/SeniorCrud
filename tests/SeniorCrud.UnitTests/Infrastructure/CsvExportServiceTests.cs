using FluentAssertions;
using Microsoft.Extensions.Options;
using SeniorCrud.Infrastructure.Csv;
using SeniorCrud.Infrastructure.Csv.Options;

namespace SeniorCrud.UnitTests.Infrastructure;

public class CsvExportServiceTests
{
    [Fact]
    public void Export_Should_Generate_Csv_With_Configured_Delimiter()
    {
        var options = Options.Create(new CsvOptions { Delimiter = ";" });
        var service = new CsvExportService(options);

        var csv = service.Export([
            new Row(Guid.Parse("11111111-1111-1111-1111-111111111111"), "Ana")
        ]);

        csv.Should().Contain("Id;Nome");
        csv.Should().Contain("Ana");
    }

    private sealed record Row(Guid Id, string Nome);
}
