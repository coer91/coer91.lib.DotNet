using System.Data;
using System.Reflection;

namespace coer91.NET
{
    public static class Clean
    {
        #region NoNesting

        /// <summary>
        /// Set navigation properties to null.
        /// </summary>
        public static List<T> NoNesting<T>(IEnumerable<T> value, string[] except = null)
            => Validations.IsNavigationProperty(typeof(T).GetType()) ? [.. value.Select(x => NoNesting(x, except))] : [.. value];


        /// <summary>
        /// Set navigation properties to null.
        /// </summary>
        public static List<T> NoNesting<T>(ICollection<T> value, string[] except = null)
            => Validations.IsNavigationProperty(typeof(T).GetType()) ? [.. value.Select(x => NoNesting(x, except))] : [.. value]; 


        /// <summary>
        /// Set navigation properties to null.
        /// </summary>
        public static T NoNesting<T>(T value, string[] except = null)
        {
            try
            {
                if (!Validations.IsNavigationProperty(value))
                    return value;

                except ??= [];  

                Type type = value.GetType();
                foreach (PropertyInfo propertyInfo in type.GetProperties())
                {
                    PropertyInfo property = type.GetProperty(propertyInfo.Name);

                    if (except.Any(x => x.Equals(propertyInfo.Name, StringComparison.OrdinalIgnoreCase)))
                    {
                        if (Validations.IsString(property))
                            property.SetValue(value, (string)property.GetValue(value));

                        continue;
                    }

                    if (!Validations.IsNavigationProperty(property) || !Validations.IsNavigationProperty(property.GetValue(value)))
                    {
                        if (Validations.IsString(property))
                            property.SetValue(value, (string)property.GetValue(value));
                    }

                    else property.SetValue(value, null);
                }

                return value;
            }

            catch
            {
                return value;
            }
        }

        #endregion


        #region NoStringEmpty 

        /// <summary>
        /// Sets null to string types if it is empty or only contains whitespace.
        /// </summary>
        public static List<T> NoStringEmpty<T>(IEnumerable<T> value, string[] except = null)
            => [.. value.Select(x => NoStringEmpty(x, except))];

        /// <summary>
        /// Sets null to string types if it is empty or only contains whitespace.
        /// </summary>
        public static List<T> NoStringEmpty<T>(ICollection<T> value, string[] except = null)
            => [.. value.Select(x => NoStringEmpty(x, except))]; 


        /// <summary>
        /// Sets null to string types if it is empty or only contains whitespace.
        /// </summary>
        public static string NoStringEmpty(string value)
            => string.IsNullOrWhiteSpace(value) ? null : value;


        /// <summary>
        /// Sets null to string types if it is empty or only contains whitespace.
        /// </summary>
        public static T NoStringEmpty<T>(T value, string[] except = null)
        {
            try
            {
                if (value is null)
                    return value;

                else if (!Validations.IsNavigationProperty(value))
                {
                    value = Validations.IsString(value) && string.IsNullOrWhiteSpace(value as string)
                        ? default : value;
                }

                else
                {
                    except ??= [];

                    Type type = value.GetType();
                    foreach (PropertyInfo propertyInfo in type.GetProperties())
                    {
                        PropertyInfo property = type.GetProperty(propertyInfo.Name);

                        if (except.Any(x => x.Equals(propertyInfo.Name, StringComparison.OrdinalIgnoreCase)))
                        {
                            if (Validations.IsString(property))
                                property.SetValue(value, (string)property.GetValue(value));

                            continue;
                        }

                        if (!Validations.IsNavigationProperty(property) || !Validations.IsNavigationProperty(property.GetValue(value)))
                        {
                            if (Validations.IsString(property) && string.IsNullOrWhiteSpace((string)property.GetValue(value)))
                                property.SetValue(value, null);
                        }
                    }
                }

                return value;
            }

            catch
            {
                return value;
            }
        }

        #endregion


        #region NoEmptyRows

        public static DataTableCollection NoEmptyRows(DataTableCollection collection)
        {
            try
            {
                if (collection is null)
                    return collection;

                DataTable table;
                for (int s = 0; s < collection.Count; s++)
                {
                    table = collection[s];
                    for (int r = table.Rows.Count - 1; r >= 0; r--)
                    {
                        if (!table.Rows[r].ItemArray.Any(cell => !string.IsNullOrWhiteSpace(cell?.ToString())))
                            table.Rows.RemoveAt(r);
                    }
                }

                return collection;
            }

            catch
            {
                return collection;
            }
        }

        #endregion
    }
}