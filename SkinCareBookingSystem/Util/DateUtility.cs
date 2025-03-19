namespace SkinCareBookingSystem.Util
{
    public static class DateUtility
    {
        private static readonly TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

        public static DateTime GetCurrentDateTime()
        {
            DateTime utcNow = DateTime.UtcNow.AddHours(7);
            return utcNow;
        }

        public static string GetCurrentDateTimeAsString(string format = "yyyy-MM-dd HH:mm")
        {
            DateTime vietnamTime = GetCurrentDateTime();
            return vietnamTime.ToString(format);
        }

        public static string GetCurrentDateAsString(string format = "yyyy-MM-dd")
        {
            DateTime vietnamTime = GetCurrentDateTime();
            return vietnamTime.ToString(format);
        }
    }
}
