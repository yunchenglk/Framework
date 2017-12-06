namespace System
{
    public static class DatetimeExt
    {
        public const string yyyyMMdd = "yyyy-MM-dd";
        public const string yyyyMMddHHmm = "yyyy-MM-dd HH:mm";


        /// <summary>格式化日期:yyyy-MM-dd HH:ss
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static string ToStandardString(this DateTime datetime)
        {
            return datetime.ToString(yyyyMMddHHmm);
        }

        /// <summary>格式化日期:yyyy-MM-dd
        /// </summary>
        /// <param name="datatime"></param>
        /// <returns></returns>
        public static string ToDate(this DateTime datatime)
        {
            return datatime.ToString(yyyyMMdd);
        }

        public static string ToStandardOrEmptyString(this DateTime datetime)
        {
            if (datetime.ToString(yyyyMMddHHmm) == "1900-01-01 00:00" || datetime.ToString(yyyyMMddHHmm) == "0001-01-01 00:00")
            {
                return "";
            }
            else
            {
                return datetime.ToString(yyyyMMddHHmm);
            }
        }
    }
}
