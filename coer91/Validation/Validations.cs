using System.Collections;
using System.Reflection;

namespace coer91
{
    public static class Validations
    {
        #region IsNotNavigationProperty

        /// <summary>
        ///  
        /// </summary>
        public static bool IsNotNavigationProperty<T>(T obj)
            => obj is not null && IsNotNavigationProperty(obj.GetType());


        /// <summary>
        ///  
        /// </summary>
        public static bool IsNotNavigationProperty(PropertyInfo property)
            => IsNotNavigationProperty(property.PropertyType);


        /// <summary>
        ///  
        /// </summary>
        public static bool IsNotNavigationProperty(Type type)
            => type == typeof(char)
            || type == typeof(string)
            || type == typeof(int)
            || type == typeof(long)
            || type == typeof(float)
            || type == typeof(double)
            || type == typeof(decimal)
            || type == typeof(bool)
            || type == typeof(DateOnly)
            || type == typeof(DateTime)
            || type == typeof(TimeOnly)
            || type == typeof(TimeSpan);

        #endregion


        #region IsString  

        /// <summary>
        /// 
        /// </summary>
        public static bool IsString<T>(T obj)
           => obj is not null && IsString(obj.GetType());


        /// <summary>
        /// 
        /// </summary>
        public static bool IsString(PropertyInfo property)
           => IsString(property.PropertyType);


        /// <summary>
        /// 
        /// </summary>
        public static bool IsString(Type type)
           => type == typeof(string);

        #endregion


        #region IsCollection

        /// <summary>
        ///  
        /// </summary>
        public static bool IsCollection(object obj)
            => obj is IEnumerable && obj is not string;

        public static bool IsCollection<T>()
            => typeof(T) != typeof(string) && typeof(IEnumerable).IsAssignableFrom(typeof(T));
        #endregion


        #region GetProperties

        /// <summary>
        /// Gets an enumerable with the object's properties. 
        /// </summary>
        public static IEnumerable<string> GetProperties(object obj)
            => IsNotNavigationProperty(obj) ? [] : obj?.GetType().GetProperties().Select(p => p.Name) ?? [];

        #endregion


        #region HasProperty

        /// <summary>
        /// Validates whether the object has the property. 
        /// </summary>
        public static bool HasProperty(string property, object obj, bool sensitive = true)
            => !IsNotNavigationProperty(obj) && (sensitive ? GetProperties(obj).Contains(property) : GetProperties(obj).Contains(property, StringComparer.OrdinalIgnoreCase));
        #endregion
    }
}