using MediatR;
using VueWebEnterprise.Application.Common;
using VueWebEnterprise.Application.DTOs;
using VueWebEnterprise.Application.Interfaces;

namespace VueWebEnterprise.Application.Features.Users.Queries
{
    public class GetUsersWithPaginationQuery : IRequest<PagedResponse<UserDto>>
    {
        public UserParameters Parameters { get; }

        public GetUsersWithPaginationQuery(UserParameters parameters)
        {
            Parameters = parameters;
        }
    }

    public class GetUsersWithPaginationQueryHandler : IRequestHandler<GetUsersWithPaginationQuery, PagedResponse<UserDto>>
    {
        private readonly IUserReadRepository _userReadRepository;

        public GetUsersWithPaginationQueryHandler(IUserReadRepository userReadRepository)
        {
            _userReadRepository = userReadRepository;
        }

        public async Task<PagedResponse<UserDto>> Handle(GetUsersWithPaginationQuery request, CancellationToken cancellationToken)
        {
            return await _userReadRepository.GetUsersAsync(request.Parameters);
        }
    }
}
