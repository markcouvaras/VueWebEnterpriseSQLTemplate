using MediatR;

namespace VueWebEnterprise.Application.Features.Users.Commands.CreateUser
{
    public record CreateUserCommand : IRequest<Guid>
    {
        public string Name { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
    }
}
