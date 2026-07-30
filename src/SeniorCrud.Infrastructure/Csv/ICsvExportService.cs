namespace SeniorCrud.Infrastructure.Csv;

public interface ICsvExportService
{
    string Export<T>(IEnumerable<T> records);
}
