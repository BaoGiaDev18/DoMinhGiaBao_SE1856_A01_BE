using System.Linq.Expressions;
using System.Reflection;

namespace PRN232.LAB.Services.Extensions
{
    /// <summary>
    /// Extension methods for IQueryable to support dynamic sorting
    /// Allows sorting by property name at runtime
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// Apply dynamic ordering to IQueryable based on property name and sort order
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="source">Source queryable</param>
        /// <param name="sortBy">Property name to sort by (case-insensitive)</param>
        /// <param name="sortOrder">"asc" or "desc"</param>
        /// <returns>Sorted queryable</returns>
        public static IQueryable<T> ApplySorting<T>(
            this IQueryable<T> source,
            string? sortBy,
            string sortOrder = "asc")
        {
            if (string.IsNullOrWhiteSpace(sortBy))
                return source;

            // Get property info (case-insensitive)
            var propertyInfo = typeof(T).GetProperty(
                sortBy,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (propertyInfo == null)
                return source; // Property not found, return unsorted

            // Build expression: x => x.PropertyName
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, propertyInfo);
            var lambda = Expression.Lambda(property, parameter);

            // Call OrderBy or OrderByDescending
            var methodName = sortOrder.ToLower() == "desc" ? "OrderByDescending" : "OrderBy";
            var method = typeof(Queryable).GetMethods()
                .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), propertyInfo.PropertyType);

            return (IQueryable<T>)method.Invoke(null, new object[] { source, lambda })!;
        }

        /// <summary>
        /// Apply pagination to IQueryable
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="source">Source queryable</param>
        /// <param name="page">Page number (1-based)</param>
        /// <param name="pageSize">Items per page</param>
        /// <returns>Paginated queryable</returns>
        public static IQueryable<T> ApplyPaging<T>(
            this IQueryable<T> source,
            int page,
            int pageSize)
        {
            return source
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }
    }
}
