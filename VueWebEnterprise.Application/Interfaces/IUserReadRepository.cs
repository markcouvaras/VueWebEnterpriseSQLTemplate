using VueWebEnterprise.Application.Common;
using VueWebEnterprise.Application.DTOs;

namespace VueWebEnterprise.Application.Interfaces
{
    public interface IUserReadRepository
    {
        Task<PagedResponse<UserDto>> GetUsersAsync(UserParameters parameters);
    }
}
