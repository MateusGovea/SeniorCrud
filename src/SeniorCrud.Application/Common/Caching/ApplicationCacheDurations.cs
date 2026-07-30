namespace SeniorCrud.Application.Common.Caching;

public static class ApplicationCacheDurations
{
    public static readonly TimeSpan Short = TimeSpan.FromMinutes(5);

    public static readonly TimeSpan Medium = TimeSpan.FromMinutes(10);

    public static readonly TimeSpan Long = TimeSpan.FromMinutes(30);
}
