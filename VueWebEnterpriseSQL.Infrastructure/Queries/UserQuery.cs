using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using VueWebEnterpriseSQL.Application.Common;
using VueWebEnterpriseSQL.Application.DTOs;
using VueWebEnterpriseSQL.Application.Interfaces;

namespace VueWebEnterpriseSQL.Infrastructure.Queries
{
    public class UserQuery : IUserReadRepository
    {
        private readonly string _connectionString;

        public UserQuery(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("DefaultConnection is not configured.");
        }

        public async Task<PagedResponse<UserDto>> GetUsersAsync(UserParameters parameters)
        {
            var skip = (parameters.Page - 1) * parameters.PageSize;
            var orderByClause = OrderQueryBuilder.BuildOrderClause<UserDto>(
                parameters.OrderBy, parameters.OrderDirection);

            var (sql, dbParams) = UserQueries.GetUsersWithFacets(
                parameters, orderByClause, skip, parameters.PageSize);

            await using var connection = new SqlConnection(_connectionString);
            await using var multi = await connection.QueryMultipleAsync(sql, dbParams);

            var totalCount = await multi.ReadSingleAsync<int>();
            var departmentFacets = (await multi.ReadAsync<TableFilterSummary<string>>()).ToList();
            var statusFacets = (await multi.ReadAsync<TableFilterSummary<int>>()).ToList();
            var items = (await multi.ReadAsync<UserDto>()).ToList();

            return new PagedResponse<UserDto>
            {
                Items = items,
                MetaData = new MetaData
                {
                    CurrentPage = parameters.Page,
                    PageSize = parameters.PageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)parameters.PageSize)
                },
                AggregatedData = new Dictionary<string, object>
                {
                    { "DepartmentTotals", departmentFacets },
                    { "StatusTotals", statusFacets }
                }
            };
        }
    }
}
