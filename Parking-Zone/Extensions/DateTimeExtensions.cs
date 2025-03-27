using System;

namespace Parking_Zone.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime StartOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, date.Kind);
        }

        public static DateTime EndOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999, date.Kind);
        }

        public static DateTime StartOfWeek(this DateTime date, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            int diff = (7 + (date.DayOfWeek - startOfWeek)) % 7;
            return date.AddDays(-1 * diff).Date;
        }

        public static DateTime EndOfWeek(this DateTime date, DayOfWeek endOfWeek = DayOfWeek.Sunday)
        {
            int diff = (7 + (endOfWeek - date.DayOfWeek)) % 7;
            return date.AddDays(diff).Date.EndOfDay();
        }

        public static DateTime StartOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1, 0, 0, 0, date.Kind);
        }

        public static DateTime EndOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month), 23, 59, 59, 999, date.Kind);
        }

        public static bool IsWeekend(this DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }

        public static bool IsWorkingDay(this DateTime date)
        {
            return !date.IsWeekend();
        }

        public static string ToRelativeTime(this DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;

            if (timeSpan <= TimeSpan.FromSeconds(60))
                return "just now";
            if (timeSpan <= TimeSpan.FromMinutes(60))
                return $"{timeSpan.Minutes} minutes ago";
            if (timeSpan <= TimeSpan.FromHours(24))
                return $"{timeSpan.Hours} hours ago";
            if (timeSpan <= TimeSpan.FromDays(30))
                return $"{timeSpan.Days} days ago";
            if (timeSpan <= TimeSpan.FromDays(365))
                return $"{timeSpan.Days / 30} months ago";

            return $"{timeSpan.Days / 365} years ago";
        }

        public static string ToShortTimeString(this TimeSpan timeSpan)
        {
            if (timeSpan.TotalDays >= 1)
                return $"{(int)timeSpan.TotalDays}d {timeSpan.Hours}h";
            if (timeSpan.TotalHours >= 1)
                return $"{(int)timeSpan.TotalHours}h {timeSpan.Minutes}m";
            if (timeSpan.TotalMinutes >= 1)
                return $"{(int)timeSpan.TotalMinutes}m";
            
            return $"{(int)timeSpan.TotalSeconds}s";
        }

        public static DateTime? Date(this DateTime? dateTime)
        {
            return dateTime?.Date;
        }

        public static TimeSpan CalculateParkingDuration(this DateTime entryTime, DateTime? exitTime = null)
        {
            exitTime ??= DateTime.Now;
            return exitTime.Value - entryTime;
        }

        public static string ToTimeZoneString(this DateTime dateTime, string timeZoneId = "SE Asia Standard Time")
        {
            try 
            {
                TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                DateTime convertedTime = TimeZoneInfo.ConvertTime(dateTime, timeZone);
                return convertedTime.ToString("yyyy-MM-dd HH:mm:ss zzz");
            }
            catch
            {
                return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        public static DateTime? ToLocalTime(this DateTime? dateTime)
        {
            return dateTime.HasValue ? dateTime.Value.ToLocalTime() : null;
        }
    }
} 