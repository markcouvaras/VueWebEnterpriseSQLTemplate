using VueWebEnterpriseSQL.Domain.Entities;

namespace VueWebEnterpriseSQL.Application.Interfaces
{
    public interface ICurrentUserService
    {
        // Returns the User entity from SQL Server, creating it if it doesn't exist (JIT)
        Task<User> GetCurrentUserAsync();
        
        // Just gets the Azure AD Object ID (OID) from the token
        Guid? GetAzureAdUserId();
    }
}