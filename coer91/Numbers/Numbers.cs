using System.Globalization; 

namespace coer91
{
    public static class Numbers
    {
        #region SetDecimals

        public static string SetDecimals(int value, int decimals = 2)
            => SetDecimals(value.ToString(), decimals);


        public static string SetDecimals(int? value, int decimals = 2)
            => value is not null ? SetDecimals(value.ToString(), decimals) : SetDecimals("0", decimals);


        public static string SetDecimals(float value, int decimals = 2)
            => SetDecimals(value.ToString(), decimals);


        public static string SetDecimals(float? value, int decimals = 2)
            => value is not null ? SetDecimals(value.ToString(), decimals) : SetDecimals("0", decimals);


        public static string SetDecimals(double value, int decimals = 2)
            => SetDecimals(value.ToString(), decimals);


        public static string SetDecimals(double? value, int decimals = 2)
            => value is not null ? SetDecimals(value.ToString(), decimals) : SetDecimals("0", decimals);


        public static string SetDecimals(decimal value, int decimals = 2)
            => SetDecimals(value.ToString(), decimals);


        public static string SetDecimals(decimal? value, int decimals = 2)
            => value is not null ? SetDecimals(value.ToString(), decimals) : SetDecimals("0", decimals);


        public static string SetDecimals(string value, int decimals = 2)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(value) || !double.TryParse(value, out double _))
                    return SetDecimals(0, decimals);

                string valueInteger = string.Empty;
                string valueDecimal = string.Empty;

                if (value.Contains('.') || decimals > 0)
                {
                    valueInteger = value.Contains('.') ? value.Split('.')[0] : value;

                    if (decimals > 0)
                    {
                        valueDecimal = value.Contains('.') ? value.Split('.')[1] : string.Empty;
                        for (int i = 0; i < decimals; i++) valueDecimal += "0";
                        valueDecimal = valueDecimal[..decimals];
                        valueDecimal = "." + valueDecimal;
                    }
                }

                else
                    valueInteger = value;

                return $"{valueInteger}{valueDecimal}";
            }

            catch
            {
                return value;
            }
        }

        #endregion


        #region ToNumericFormat

        public static string ToNumericFormat(int value, int decimals = 0)
           => ToNumericFormat(value.ToString(), decimals);


        public static string ToNumericFormat(int? value, int decimals = 0)
           => value is not null ? ToNumericFormat(value.ToString(), decimals) : SetDecimals("0", decimals);

        public static string ToNumericFormat(float value, int decimals = 2)
          => ToNumericFormat(value.ToString(), decimals);


        public static string ToNumericFormat(float? value, int decimals = 2)
           => value is not null ? ToNumericFormat(value.ToString(), decimals) : SetDecimals("0", decimals);


        public static string ToNumericFormat(double value, int decimals = 2)
          => ToNumericFormat(value.ToString(), decimals);


        public static string ToNumericFormat(double? value, int decimals = 2)
           => value is not null ? ToNumericFormat(value.ToString(), decimals) : SetDecimals("0", decimals);


        public static string ToNumericFormat(decimal value, int decimals = 2)
            => ToNumericFormat(value.ToString(), decimals);


        public static string ToNumericFormat(decimal? value, int decimals = 2)
            => value is not null ? ToNumericFormat(value.ToString(), decimals) : SetDecimals("0", decimals);


        public static string ToNumericFormat(string value, int decimals = 0)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(value) || !double.TryParse(value, out double _))
                    return SetDecimals(0, decimals);

                value = SetDecimals(value, decimals);
                string valueInteger = string.Empty;
                string valueDecimal = string.Empty;

                if (decimals > 0)
                {
                    valueInteger = value.Split('.')[0];
                    valueDecimal = value.Split('.')[1];
                    valueDecimal = "." + valueDecimal;
                }

                else
                    valueInteger = value;

                valueInteger = long.Parse(valueInteger).ToString("N0", CultureInfo.InvariantCulture);
                return $"{valueInteger}{valueDecimal}";
            }

            catch  
            {
                return value;
            }
        }

        #endregion


        #region ToCurrencyFormat
        public static string ToCurrencyFormat(int value, char symbol = '$', string currency = "")
           => ToCurrencyFormat(value.ToString(), symbol, currency);


        public static string ToCurrencyFormat(int? value, char symbol = '$', string currency = "")
           => value is not null ? ToCurrencyFormat(value.ToString(), symbol, currency) : $"{symbol}0.00{(!string.IsNullOrWhiteSpace(currency) ? $" {currency}" : string.Empty)}";

        public static string ToCurrencyFormat(float value, char symbol = '$', string currency = "")
          => ToCurrencyFormat(value.ToString(), symbol, currency);


        public static string ToCurrencyFormat(float? value, char symbol = '$', string currency = "")
           => value is not null ? ToCurrencyFormat(value.ToString(), symbol, currency) : $"{symbol}0.00{(!string.IsNullOrWhiteSpace(currency) ? $" {currency}" : string.Empty)}";


        public static string ToCurrencyFormat(double value, char symbol = '$', string currency = "")
          => ToCurrencyFormat(value.ToString(), symbol, currency);


        public static string ToCurrencyFormat(double? value, char symbol = '$', string currency = "")
           => value is not null ? ToCurrencyFormat(value.ToString(), symbol, currency) : $"{symbol}0.00{(!string.IsNullOrWhiteSpace(currency) ? $" {currency}" : string.Empty)}";


        public static string ToCurrencyFormat(decimal value, char symbol = '$', string currency = "")
            => ToCurrencyFormat(value.ToString(), symbol, currency);


        public static string ToCurrencyFormat(decimal? value, char symbol = '$', string currency = "")
            => value is not null ? ToCurrencyFormat(value.ToString(), symbol, currency) : $"{symbol}0.00{(!string.IsNullOrWhiteSpace(currency) ? $" {currency}" : string.Empty)}";


        public static string ToCurrencyFormat(string value, char symbol = '$', string currency = "")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(value) || !double.TryParse(value, out double _))
                    return $"{symbol}0.00{(!string.IsNullOrWhiteSpace(currency) ? $" {currency}" : string.Empty)}";

                return symbol + ToNumericFormat(value, 2) + (!string.IsNullOrWhiteSpace(currency) ? $" {currency}" : string.Empty);
            }

            catch 
            {
                return value;
            }
        }

        #endregion
    }
} 