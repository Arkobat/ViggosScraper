namespace ViggosScraper.Service;

public class DateFormatter
{
    public static DateTime MapViggosDate(string dateString)
    {
        var dateTime = DateTime.ParseExact(dateString, "dd-MM-yyyy HH:mm", null);
        return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
    }
}