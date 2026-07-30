using System.Diagnostics;

namespace SeniorCrud.Application.Common.Diagnostics;

public static class ApplicationActivitySource
{
    public const string SourceName = "SeniorCrud.Application";

    public static readonly ActivitySource Source = new(SourceName);
}
