namespace VueWebEnterprise.Application.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Department { get; set; }
        public string? Status { get; set; }
        public DateTime LastLoginAt { get; set; }
    }
}
