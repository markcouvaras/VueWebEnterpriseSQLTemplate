using System.ComponentModel.DataAnnotations;

namespace VueWebEnterprise.Domain.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        // We link this to Azure AD Object ID (The "Shadow User" concept)
        public Guid AzureAdUserId { get; set; }

        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        
        public DateTime LastLoginAt { get; set; }
    }
}