namespace coer91.NET
{
    public static class NumbersExtension
    {
        #region SetDecimals

        public static string SetDecimals(this int value, int decimals = 2)
            => Numbers.SetDecimals(value, decimals);

        public static string SetDecimals(this float value, int decimals = 2)
            => Numbers.SetDecimals(value, decimals);

        public static string SetDecimals(this double value, int decimals = 2)
            => Numbers.SetDecimals(value, decimals);

        public static string SetDecimals(this decimal value, int decimals = 2)
            => Numbers.SetDecimals(value, decimals);

        #endregion


        #region ToNumericFormat

        public static string ToNumericFormat(this int value, int decimals = 0)
           => Numbers.ToNumericFormat(value, decimals);

        public static string ToNumericFormat(this float value, int decimals = 2)
            => Numbers.ToNumericFormat(value, decimals);

        public static string ToNumericFormat(this double value, int decimals = 2)
            => Numbers.ToNumericFormat(value, decimals);

        public static string ToNumericFormat(this decimal value, int decimals = 2)
            => Numbers.ToNumericFormat(value, decimals);

        #endregion


        #region ToCurrencyFormat

        public static string ToCurrencyFormat(this int value, char symbol = '$', string currency = "")
           => Numbers.ToCurrencyFormat(value, symbol, currency);

        public static string ToCurrencyFormat(this float value, char symbol = '$', string currency = "")
            => Numbers.ToCurrencyFormat(value, symbol, currency);

        public static string ToCurrencyFormat(this double value, char symbol = '$', string currency = "")
            => Numbers.ToCurrencyFormat(value, symbol, currency);

        public static string ToCurrencyFormat(this decimal value, char symbol = '$', string currency = "")
            => Numbers.ToCurrencyFormat(value, symbol, currency);

        #endregion


        #region ToDateTime

        public static DateTime? ToDateTime(this int? value, string timeZone = "Central Standard Time", bool isUtc = true)
           => Dates.ToDateTime(value, timeZone, isUtc);

        public static DateTime ToDateTime(this int value, string timeZone = "Central Standard Time", bool isUtc = true)
            => Dates.ToDateTime(value, timeZone, isUtc);

        public static DateTime? ToDateTime(this float? value, string timeZone = "Central Standard Time", bool isUtc = true)
           => Dates.ToDateTime(value, timeZone, isUtc);

        public static DateTime ToDateTime(this float value, string timeZone = "Central Standard Time", bool isUtc = true)
            => Dates.ToDateTime(value, timeZone, isUtc);

        public static DateTime? ToDateTime(this double? value, string timeZone = "Central Standard Time", bool isUtc = true)
           => Dates.ToDateTime(value, timeZone, isUtc);

        public static DateTime ToDateTime(this double value, string timeZone = "Central Standard Time", bool isUtc = true)
            => Dates.ToDateTime(value, timeZone, isUtc);

        public static DateTime? ToDateTime(this decimal? value, string timeZone = "Central Standard Time", bool isUtc = true)
           => Dates.ToDateTime(value, timeZone, isUtc);

        public static DateTime ToDateTime(this decimal value, string timeZone = "Central Standard Time", bool isUtc = true)
            => Dates.ToDateTime(value, timeZone, isUtc);

        public static DateTime? ToDateTime(this long? value, string timeZone = "Central Standard Time", bool isUtc = true)
            => Dates.ToDateTime(value, timeZone, isUtc);

        public static DateTime ToDateTime(this long value, string timeZone = "Central Standard Time", bool isUtc = true)
            => Dates.ToDateTime(value, timeZone, isUtc);

        #endregion
    }
}