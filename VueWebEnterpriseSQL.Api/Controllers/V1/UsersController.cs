using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using VueWebEnterpriseSQL.Application.Common;
using VueWebEnterpriseSQL.Application.DTOs;
using VueWebEnterpriseSQL.Application.Features.Users.Queries;
using VueWebEnterpriseSQL.Application.Interfaces;
using VueWebEnterpriseSQL.Domain.Entities;
using VueWebEnterpriseSQL.Infrastructure.Data;

namespace VueWebEnterpriseSQL.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    public class UsersController : BaseController
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public UsersController(AppDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        // GET: api/v1/users?page=1&pageSize=10&searchTerm=john&orderBy=FullName&orderDirection=desc&department=Engineering
        // Returns a paginated, filtered, sortable list of users with faceted aggregations (Dapper read-side)
        [HttpGet]
        public async Task<ActionResult<PagedResponse<UserDto>>> GetUsers([FromQuery] UserParameters parameters)
        {
            var result = await Mediator.Send(new GetUsersWithPaginationQuery(parameters));
            return Ok(result);
        }

        // GET: api/v1/users/me
        // Returns the currently logged-in user (Creates them if they don't exist!)
        // [Authorize] // <-- Uncomment this once you have real Entra ID tokens flowing
        [HttpGet("me")]
        public async Task<ActionResult<User>> GetMe()
        {
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                return Ok(user);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("No valid token found. Are you logged in?");
            }
        }

        // Dummy endpoint to return a fake list of users for testing
        [HttpGet("dummy")]
        public async Task<ActionResult<User>> GetDummy()
        {
            var dummyUsers = new List<User>
            {
                new User { Id = Guid.NewGuid(), FullName = "dummyuser1", Email = "dummy1@example.com" },
                new User { Id = Guid.NewGuid(), FullName = "dummyuser2", Email = "dummy2@example.com" },
                new User { Id = Guid.NewGuid(), FullName = "dummyuser3", Email = "dummy3@example.com" },
            };
            return Ok(dummyUsers);
        }
    }
}
