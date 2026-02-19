using VueWebEnterpriseSQL.Application.Common;
using VueWebEnterpriseSQL.Application.DTOs;

namespace VueWebEnterpriseSQL.Application.Interfaces
{
    public interface IUserReadRepository
    {
        Task<PagedResponse<UserDto>> GetUsersAsync(UserParameters parameters);
    }
}
