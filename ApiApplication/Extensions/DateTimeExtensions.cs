using System;

namespace ApiApplication
{
    public static class DateTimeExtensions
    {
        public static bool IsExpired(this DateTime dateTime, int minute = 10)
        {
            return dateTime.AddMinutes(minute) < DateTime.Now;
        }
    }
}