namespace Domain;

public static class EgyptTime
{
    private static readonly TimeZoneInfo Zone = TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo");

    // DATABASE: Always UTC 
    public static DateTimeOffset UtcNow => DateTimeOffset.UtcNow;

    // DISPLAY/LOGGING: Egypt time (Offset = +02:00 or +03:00)
    public static DateTimeOffset EgyptNow => TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, Zone);

    // Convert DB values to Egypt time for responses
    public static DateTimeOffset ToEgypt(DateTimeOffset utc) => TimeZoneInfo.ConvertTime(utc, Zone);

    public static DateTime NowDateTimeUnspecified => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Zone);
}
