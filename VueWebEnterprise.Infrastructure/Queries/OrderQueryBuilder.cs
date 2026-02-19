using System.Reflection;

namespace VueWebEnterprise.Infrastructure.Queries
{
    /// <summary>
    /// Securely maps client-supplied OrderBy + OrderDirection to a SQL ORDER BY clause.
    /// Uses reflection to validate that OrderBy matches an actual property on TEntity,
    /// preventing both SQL injection and invalid column errors.
    /// </summary>
    public static class OrderQueryBuilder
    {
        public static string BuildOrderClause<TEntity>(
            string? orderBy,
            string? orderDirection,
            string defaultSort = "[Id] ASC")
        {
            if (string.IsNullOrWhiteSpace(orderBy))
                return defaultSort;

            var property = typeof(TEntity)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(p => p.Name.Equals(orderBy, StringComparison.OrdinalIgnoreCase));

            if (property is null)
                return defaultSort;

            var direction = orderDirection?.Equals("desc", StringComparison.OrdinalIgnoreCase) == true
                ? "DESC"
                : "ASC";

            return $"[{property.Name}] {direction}";
        }
    }
}
