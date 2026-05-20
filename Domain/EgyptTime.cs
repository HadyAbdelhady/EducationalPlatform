namespace Domain;

public static class EgyptTime
{
    private static readonly TimeZoneInfo Zone = TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo");

    public static DateTimeOffset Now => TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, Zone);

    public static DateTime NowDateTimeUnspecified => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Zone);
}
