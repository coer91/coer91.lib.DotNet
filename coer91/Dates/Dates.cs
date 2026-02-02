using System.Globalization;

namespace coer91
{
    public static class Dates
    {
        #region IsValidDateTime
        public static bool IsValidDateTime(string dateTime)
            => DateTime.TryParse(dateTime, out _);


        public static bool IsValidDateTime(DateTime? dateTime)
            => dateTime is not null;
        #endregion


        #region GetCurrentDateTime
        public static DateTime GetCurrentDateTime()
            => DateTime.Now;


        public static DateOnly GetCurrentDate()
            => DateOnly.FromDateTime(GetCurrentDateTime()); 
        #endregion


        #region GetCurrentDateTimeUTC 
        public static DateTime GetCurrentDateTimeUTC()
            => DateTime.UtcNow;


        public static DateOnly GetCurrentDateUTC()
            => DateOnly.FromDateTime(GetCurrentDateTimeUTC());
        #endregion


        #region ToFormatTime

        /// <summary>
        /// HH:mm/hh:mm tt
        /// </summary> 
        public static string ToFormatTime(string dateTime, bool ampm = true)
            => IsValidDateTime(dateTime) ? ToFormatTime(DateTime.Parse(dateTime), ampm) : string.Empty;

        /// <summary>
        /// HH:mm/hh:mm tt
        /// </summary> 
        public static string ToFormatTime(DateTime? dateTime, bool ampm = true)
            => IsValidDateTime(dateTime) ? ToFormatTime((DateTime)dateTime, ampm) : string.Empty;

        /// <summary>
        /// HH:mm/hh:mm tt
        /// </summary> 
        public static string ToFormatTime(DateTime dateTime, bool ampm = true)
            => ampm 
            ? dateTime.ToString("hh:mm tt", CultureInfo.InvariantCulture).Replace("AM", "am").Replace("PM", "pm")
            : dateTime.ToString("HH:mm", CultureInfo.InvariantCulture);

        #endregion


        #region ToFormatMDY

        /// <summary>
        /// MMM dd, yyyy
        /// </summary>
        public static string ToFormatMDY(string dateTime)
            => IsValidDateTime(dateTime) ? ToFormatMDY(DateTime.Parse(dateTime)) : string.Empty;

        /// <summary>
        /// MMM dd, yyyy
        /// </summary>
        public static string ToFormatMDY(DateTime? dateTime)
            => IsValidDateTime(dateTime) ? ToFormatMDY((DateTime)dateTime) : string.Empty;

        /// <summary>
        /// MMM dd, yyyy
        /// </summary>
        public static string ToFormatMDY(DateTime dateTime)
            => dateTime.ToString("MMM dd, yyyy", CultureInfo.InvariantCulture);

        #endregion


        #region ToFormatMDYTime

        /// <summary>
        /// MMM dd, yyyy at HH:mm/hh:mm tt
        /// </summary>
        public static string ToFormatMDYTime(string dateTime, bool ampm = true)
            => IsValidDateTime(dateTime) ? ToFormatMDYTime(DateTime.Parse(dateTime), ampm) : string.Empty;

        /// <summary>
        /// MMM dd, yyyy at HH:mm/hh:mm tt
        /// </summary>
        public static string ToFormatMDYTime(DateTime? dateTime, bool ampm = true)
            => IsValidDateTime(dateTime) ? ToFormatMDYTime((DateTime)dateTime, ampm) : string.Empty;

        /// <summary>
        /// MMM dd, yyyy at HH:mm/hh:mm tt
        /// </summary>
        public static string ToFormatMDYTime(DateTime dateTime, bool ampm = true)
            => ToFormatMDY(dateTime) + " at " + ToFormatTime(dateTime, ampm);

        #endregion


        #region ToFormatDMY

        /// <summary>
        /// dd MMM yyyy
        /// </summary>
        public static string ToFormatDMY(string dateTime)
            => IsValidDateTime(dateTime) ? ToFormatDMY(DateTime.Parse(dateTime)) : string.Empty;

        /// <summary>
        /// dd MMM yyyy
        /// </summary>
        public static string ToFormatDMY(DateTime? dateTime)
            => IsValidDateTime(dateTime) ? ToFormatDMY((DateTime)dateTime) : string.Empty;

        /// <summary>
        /// dd MMM yyyy
        /// </summary>
        public static string ToFormatDMY(DateTime dateTime)
            => dateTime.ToString("dd MMM yyyy", CultureInfo.InvariantCulture);

        #endregion


        #region ToFormatDMYTime

        /// <summary>
        /// dd MMM yyyy at HH:mm/hh:mm tt
        /// </summary>
        public static string ToFormatDMYTime(string dateTime, bool ampm = true)
            => IsValidDateTime(dateTime) ? ToFormatDMYTime(DateTime.Parse(dateTime), ampm) : string.Empty;

        /// <summary>
        /// dd MMM yyyy at HH:mm/hh:mm tt
        /// </summary>
        public static string ToFormatDMYTime(DateTime? dateTime, bool ampm = true)
            => IsValidDateTime(dateTime) ? ToFormatDMYTime((DateTime)dateTime, ampm) : string.Empty;

        /// <summary>
        /// dd MMM yyyy at HH:mm/hh:mm tt
        /// </summary>
        public static string ToFormatDMYTime(DateTime dateTime, bool ampm = true)
            => ToFormatDMY(dateTime) + " at " + ToFormatTime(dateTime, ampm);

        #endregion


        #region SetFirstHour

        /// <summary>
        /// Set 00:00:00 
        /// </summary> 
        public static DateTime? SetFirstHour(string dateTime)
            => IsValidDateTime(dateTime) ? SetFirstHour(DateTime.Parse(dateTime)) : null;


        /// <summary>
        /// Set 00:00:00 
        /// </summary> 
        public static DateTime? SetFirstHour(DateTime? dateTime)
            => IsValidDateTime(dateTime) ? SetFirstHour((DateTime)dateTime) : null;


        /// <summary>
        /// Set 00:00:00
        /// </summary> 
        public static DateTime SetFirstHour(DateTime dateTime)
            => dateTime.AddHours(-dateTime.Hour).AddMinutes(-dateTime.Minute).AddSeconds(-dateTime.Second).AddMilliseconds(-dateTime.Millisecond);

        #endregion


        #region SetLastHour

        /// <summary>
        /// Set 23:59:59 
        /// </summary> 
        public static DateTime? SetLastHour(string dateTime)
            => IsValidDateTime(dateTime) ? SetLastHour(DateTime.Parse(dateTime)) : null;


        /// <summary>
        /// Set 23:59:59 
        /// </summary> 
        public static DateTime? SetLastHour(DateTime? dateTime)
            => IsValidDateTime(dateTime) ? SetLastHour((DateTime)dateTime) : null;


        /// <summary>
        /// Set 23:59:59
        /// </summary> 
        public static DateTime SetLastHour(DateTime dateTime)
            => SetFirstHour(dateTime).AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(59);

        #endregion


        #region ToDateTime 
        public static DateTime? ToDateTime(string dateTime)
            => IsValidDateTime(dateTime) ? DateTime.Parse(dateTime) : null;

        public static DateTime ToDateTime(int timestamp, string timeZone = "Central Standard Time", bool isUtc = true)
           => TimestampToDateTime(timestamp, timeZone, isUtc);

        public static DateTime? ToDateTime(int? timestamp, string timeZone = "Central Standard Time", bool isUtc = true)
            => timestamp is not null ? ToDateTime((int)timestamp, timeZone, isUtc) : null;

        public static DateTime ToDateTime(float timestamp, string timeZone = "Central Standard Time", bool isUtc = true)
            => TimestampToDateTime((long)Math.Round(timestamp), timeZone, isUtc);

        public static DateTime? ToDateTime(float? timestamp, string timeZone = "Central Standard Time", bool isUtc = true)
            => timestamp is not null ? ToDateTime((float)timestamp, timeZone, isUtc) : null;

        public static DateTime ToDateTime(double timestamp, string timeZone = "Central Standard Time", bool isUtc = true)
            => TimestampToDateTime((long)Math.Round(timestamp), timeZone, isUtc);

        public static DateTime? ToDateTime(double? timestamp, string timeZone = "Central Standard Time", bool isUtc = true)
            => timestamp is not null ? ToDateTime((double)timestamp, timeZone, isUtc) : null;

        public static DateTime ToDateTime(decimal timestamp, string timeZone = "Central Standard Time", bool isUtc = true)
            => TimestampToDateTime((long)Math.Round(timestamp), timeZone, isUtc);

        public static DateTime? ToDateTime(decimal? timestamp, string timeZone = "Central Standard Time", bool isUtc = true)
            => timestamp is not null ? ToDateTime((decimal)timestamp, timeZone, isUtc) : null;

        public static DateTime ToDateTime(long timestamp, string timeZone = "Central Standard Time", bool isUtc = true)
            => TimestampToDateTime(timestamp, timeZone, isUtc);

        public static DateTime? ToDateTime(long? timestamp, string timeZone = "Central Standard Time", bool isUtc = true)
            => timestamp is not null ? TimestampToDateTime((long)timestamp, timeZone, isUtc) : null; 

        private static DateTime TimestampToDateTime(long timestamp, string timeZone, bool isUtc = true)
        {
            try
            {
                DateTimeOffset baseTime = DateTimeOffset.FromUnixTimeSeconds(timestamp);
                TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZone);

                if (isUtc)
                {
                    DateTime utcDateTime = baseTime.UtcDateTime;
                    DateTime localDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZoneInfo);
                    return DateTime.SpecifyKind(localDateTime, DateTimeKind.Local);
                }

                else
                {
                    DateTimeOffset localOffset = TimeZoneInfo.ConvertTime(baseTime, timeZoneInfo);
                    return DateTime.SpecifyKind(localOffset.DateTime, DateTimeKind.Local);
                }
            }

            catch (TimeZoneNotFoundException)
            {
                throw new ArgumentException($"Zona horaria no válida: '{timeZone}'", nameof(timeZone));
            }
        }
        #endregion


        #region ToDateOnly
        public static DateOnly? ToDateOnly(string dateTime)
            => IsValidDateTime(dateTime) ? ToDateOnly(DateTime.Parse(dateTime)) : null;

        public static DateOnly? ToDateOnly(DateTime? dateTime)
            => IsValidDateTime(dateTime) ? ToDateOnly((DateTime)dateTime) : null;

        public static DateOnly ToDateOnly(DateTime dateTime)
           => DateOnly.FromDateTime(dateTime); 
        #endregion
    }
}