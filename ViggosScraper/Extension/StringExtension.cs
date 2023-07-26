namespace ViggosScraper.Extension;

public static class StringExtension
{
    public static string DigitOnly(this string str)
    {
        return str.Where(char.IsDigit).Aggregate("", (s, c) => s + c);
    }
}