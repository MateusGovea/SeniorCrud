namespace SeniorCrud.Application.Abstractions.Csv;

public interface ICsvExportService
{
    string Export<T>(IEnumerable<T> records);
}
