using System;
using System.Globalization;
using System.Linq;

namespace DamianTourBackend.Application
{
    static class DateParser
    {
        public static string Parse(string dateString)
        {
            if (dateString == null) return null;
            var normalizedDate = string.Join("", dateString.Where(char.IsDigit));
            return DateTime.ParseExact(normalizedDate, "ddMMyyyy", CultureInfo.GetCultureInfo("nl-BE")).ToString("dd-MM-yyyy");
        }

        public static bool TryParse(string dateString, out DateTime date)
        {
            var normalizedDate = string.Join("", dateString.Where(char.IsDigit));
            return DateTime.TryParseExact(normalizedDate, "ddMMyyyy", CultureInfo.GetCultureInfo("nl-BE"), DateTimeStyles.None, out date);
        }
    }
}