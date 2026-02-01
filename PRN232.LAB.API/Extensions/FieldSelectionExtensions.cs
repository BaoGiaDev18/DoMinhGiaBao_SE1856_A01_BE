using System.Dynamic;
using System.Reflection;
using System.Text.Json;

namespace DoMinhGiaBao__SE1856_A01_BE.Extensions
{
    /// <summary>
    /// Extension methods for dynamic field selection
    /// Allows clients to request only specific fields in the response
    /// </summary>
    public static class FieldSelectionExtensions
    {
        /// <summary>
        /// Apply field selection to a collection of objects
        /// </summary>
        /// <typeparam name="T">Source object type</typeparam>
        /// <param name="source">Source collection</param>
        /// <param name="fields">Comma-separated field names (e.g., "id,name,email")</param>
        /// <returns>Collection of dynamic objects with only requested fields</returns>
        public static IEnumerable<object> SelectFields<T>(
            this IEnumerable<T> source,
            string? fields)
        {
            if (string.IsNullOrWhiteSpace(fields))
                return source.Cast<object>(); // Return all fields

            var fieldList = fields
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(f => f.Trim())
                .ToList();

            if (!fieldList.Any())
                return source.Cast<object>();

            return source.Select(item => SelectFieldsFromObject(item, fieldList));
        }

        /// <summary>
        /// Select specific fields from a single object
        /// </summary>
        private static object SelectFieldsFromObject<T>(T item, List<string> fields)
        {
            var expandoObject = new ExpandoObject() as IDictionary<string, object?>;
            var type = typeof(T);

            foreach (var field in fields)
            {
                // Find property (case-insensitive)
                var property = type.GetProperty(
                    field,
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (property != null && item != null)
                {
                    var value = property.GetValue(item);
                    // Use original property name (PascalCase) for consistency
                    var propertyName = char.ToLowerInvariant(property.Name[0]) + property.Name.Substring(1);
                    expandoObject[propertyName] = value;
                }
            }

            return expandoObject;
        }
    }
}
