namespace coer91.NET
{
    public static class DatesExtension
    { 
        /// <summary>
        /// HH:mm/hh:mm tt
        /// </summary> 
        public static string ToFormatTime(this DateTime dateTime, bool ampm = true)
            => Dates.ToFormatTime(dateTime, ampm);


        /// <summary>
        /// MMM dd, yyyy
        /// </summary> 
        public static string ToFormatMDY(this DateTime dateTime)
            => Dates.ToFormatMDY(dateTime);


        /// <summary>
        /// MMM dd, yyyy at HH:mm/hh:mm tt
        /// </summary> 
        public static string ToFormatMDYTime(this DateTime dateTime, bool ampm = true)
            => Dates.ToFormatMDYTime(dateTime, ampm);


        /// <summary>
        /// dd MMM yyyy
        /// </summary> 
        public static string ToFormatDMY(this DateTime dateTime)
            => Dates.ToFormatDMY(dateTime);


        /// <summary>
        /// dd MMM yyyy at HH:mm/hh:mm tt
        /// </summary> 
        public static string ToFormatDMYTime(this DateTime dateTime)
            => Dates.ToFormatDMYTime(dateTime);


        /// <summary>
        /// Set 00:00:00
        /// </summary> 
        public static DateTime SetFirstHour(this DateTime dateTime)
            => Dates.SetFirstHour(dateTime);


        /// <summary>
        /// Set 23:59:59
        /// </summary> 
        public static DateTime SetLastHour(this DateTime dateTime)
            => Dates.SetLastHour(dateTime);


        /// <summary>
        ///  
        /// </summary> 
        public static DateOnly ToDateOnly(this DateTime date)
            => Dates.ToDateOnly(date);
    }
}