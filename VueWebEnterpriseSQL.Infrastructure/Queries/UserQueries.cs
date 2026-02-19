using Dapper;
using VueWebEnterpriseSQL.Application.Common;
using VueWebEnterpriseSQL.Application.DTOs;

namespace VueWebEnterpriseSQL.Infrastructure.Queries
{
    /// <summary>
    /// Static query factory for User read operations.
    /// All SQL lives here — the repository only executes and maps results.
    /// </summary>
    public static class UserQueries
    {
        /// <summary>
        /// Builds a single CTE query that returns four result sets in one round-trip:
        /// 1) Total count, 2) Department facets, 3) Status facets, 4) Paginated data.
        /// Uses T-SQL IN (SELECT value FROM STRING_SPLIT) for multi-value filter support.
        /// </summary>
        public static (string Sql, DynamicParameters Parameters) GetUsersWithFacets(
            UserParameters parameters, string orderByClause, int skip, int take)
        {
            var dbParams = new DynamicParameters();
            var conditions = new List<string>();

            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                conditions.Add(@"([FullName] LIKE @Search OR [Email] LIKE @Search)");
                dbParams.Add("Search", $"%{parameters.SearchTerm}%");
            }

            if (parameters.Departments is { Count: > 0 })
            {
                conditions.Add(@"[Department] IN @Departments");
                dbParams.Add("Departments", parameters.Departments);
            }

            if (parameters.Statuses is { Count: > 0 })
            {
                conditions.Add(@"[Status] IN @Statuses");
                dbParams.Add("Statuses", parameters.Statuses);
            }

            dbParams.Add("Skip", skip);
            dbParams.Add("Take", take);

            var whereClause = conditions.Count > 0
                ? "WHERE " + string.Join(" AND ", conditions)
                : string.Empty;

            var sql = $@"
                ;WITH filtered AS (
                    SELECT * FROM [Users] {whereClause}
                )

                SELECT COUNT(*) FROM filtered;

                SELECT [Department] AS [Value], CAST(COUNT(*) AS int) AS [Count]
                FROM filtered
                WHERE [Department] IS NOT NULL
                GROUP BY [Department]
                ORDER BY [Count] DESC;

                SELECT CAST([Status] AS int) AS [Value], CAST(COUNT(*) AS int) AS [Count]
                FROM filtered
                WHERE [Status] IS NOT NULL
                GROUP BY [Status]
                ORDER BY [Count] DESC;

                SELECT [Id], [Email], [FullName], [Department], [Status], [LastLoginAt]
                FROM filtered
                ORDER BY {orderByClause}
                OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;";

            return (sql, dbParams);
        }
    }
}
