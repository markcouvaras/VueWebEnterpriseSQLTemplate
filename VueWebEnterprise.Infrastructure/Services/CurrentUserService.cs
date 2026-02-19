using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using VueWebEnterprise.Application.Interfaces;
using VueWebEnterprise.Domain.Entities;
using VueWebEnterprise.Infrastructure.Data;

namespace VueWebEnterprise.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _context;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor, AppDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public Guid? GetAzureAdUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var oid = user?.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value 
                      ?? user?.FindFirst("oid")?.Value;

            return oid != null ? Guid.Parse(oid) : null;
        }

        public async Task<User> GetCurrentUserAsync()
        {
            var azureId = GetAzureAdUserId();

            if (azureId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated with Azure AD.");
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.AzureAdUserId == azureId.Value);

            if (user == null)
            {
                var claims = _httpContextAccessor.HttpContext?.User;
                user = new User
                {
                    Id = Guid.NewGuid(),
                    AzureAdUserId = azureId.Value,
                    Email = claims?.FindFirst("preferred_username")?.Value ?? "unknown@email.com",
                    FullName = claims?.FindFirst("name")?.Value ?? "Unknown User",
                    LastLoginAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            return user;
        }
    }
}