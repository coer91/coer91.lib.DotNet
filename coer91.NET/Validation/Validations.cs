using System.Collections;
using System.Reflection;

namespace coer91.NET
{
    public static class Validations
    {
        #region IsNavigationProperty

        /// <summary>
        ///  
        /// </summary>
        public static bool IsNavigationProperty<T>(T obj)
            => obj is not null && IsNavigationProperty(obj.GetType());


        /// <summary>
        ///  
        /// </summary>
        public static bool IsNavigationProperty(PropertyInfo property)
            => IsNavigationProperty(property.PropertyType);


        /// <summary>
        ///  
        /// </summary>
        public static bool IsNavigationProperty(Type type)
        { 
            return (type is not null) && !(
                   type == typeof(char)
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
                || type == typeof(TimeSpan)
            );

        }

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
           => property is not null && IsString(property.PropertyType);


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
            => obj is not null && obj is IEnumerable && obj is not string;

        public static bool IsCollection<T>()
            => typeof(T) != typeof(string) && typeof(IEnumerable).IsAssignableFrom(typeof(T));
        #endregion


        #region GetProperties

        /// <summary>
        /// Gets an enumerable with the object's properties. 
        /// </summary>
        public static IEnumerable<string> GetProperties(object obj)
        {
            try
            {
                if (IsNavigationProperty(obj))
                {
                    Type type = obj?.GetType();

                    if (type.Name.Equals("JObject") && type.FullName.Equals("Newtonsoft.Json.Linq.JObject"))
                    {
                        dynamic objDynamic = obj;
                        IEnumerable<dynamic> properties = objDynamic.Properties();
                        return properties.Select(x => $"{x.Name}");
                    }

                    return type?.GetProperties()?.Select(x => x.Name) ?? [];
                }
            }
            catch { }
            return [];
        }

        #endregion


        #region HasProperty

        /// <summary>
        /// Validates whether the object has the property. 
        /// </summary>
        public static bool HasProperty(string property, object obj, bool sensitive = true)
            => IsNavigationProperty(obj) && (sensitive ? GetProperties(obj).Contains(property) : GetProperties(obj).Contains(property, StringComparer.OrdinalIgnoreCase));
        #endregion
    }
}