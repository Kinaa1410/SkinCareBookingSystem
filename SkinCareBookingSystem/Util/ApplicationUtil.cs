namespace SkinCareBookingSystem.Util
{
    public static class ApplicationUtil
    {
        public static long GetNewID()
        {
            long id = long.Parse(DateUtility.GetCurrentDateTimeAsString("yyyyMMddHHmmssffff")) - 200000000000000000;

            return id;
        }
    }
}
