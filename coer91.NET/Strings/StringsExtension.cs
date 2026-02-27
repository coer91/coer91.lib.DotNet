namespace coer91.NET
{
    public static class StringsExtension
    { 
        /// <summary>
        /// Removes extra spaces between words
        /// </summary> 
        public static string CleanUpBlanks(this string value)
            => Strings.CleanUpBlanks(value);


        /// <summary>
        /// 
        /// </summary>
        public static string FirstCharToUpper(this string value)
            => Strings.FirstCharToUpper(value);


        /// <summary>
        /// 
        /// </summary>
        public static string FirstCharToLower(this string value)
            => Strings.FirstCharToLower(value);


        /// <summary>
        /// 
        /// </summary>
        public static string ToPascalCase(this string value)
            => Strings.ToPascalCase(value);


        /// <summary>
        /// 
        /// </summary>
        public static string ToCamelCase(this string value)
            => Strings.ToCamelCase(value);


        /// <summary>
        /// 
        /// </summary>
        public static string ToSnakeCase(this string value)
            => Strings.ToSnakeCase(value);


        /// <summary>
        /// 
        /// </summary>
        public static string ToScreamingSnakeCase(this string value)
            => Strings.ToScreamingSnakeCase(value);


        /// <summary>
        /// 
        /// </summary>
        public static string ToKebabCase(this string value)
            => Strings.ToKebabCase(value);


        /// <summary>
        /// 
        /// </summary>
        public static string RemoveLastChar(this string value)
            => Strings.RemoveLastChar(value);


        /// <summary>
        /// 
        /// </summary>
        public static string RemoveAccents(this string value)
            => Strings.RemoveAccents(value); 
                     

        /// <summary>
        /// 
        /// </summary> 
        public static DateTime? ToDateTime(this string value)
            => Dates.ToDateTime(value);


        /// <summary>
        /// 
        /// </summary> 
        public static DateOnly? ToDateOnly(this string value)
            => Dates.ToDateOnly(value);


        /// <summary>
        /// 
        /// </summary> 
        public static string ConcatName(this string value, params string[] args)
            => Strings.ConcatName([value, .. args]); 
    }
}