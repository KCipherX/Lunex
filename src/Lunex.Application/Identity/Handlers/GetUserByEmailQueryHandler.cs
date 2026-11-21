using Lunex.Application.Identity.Queries;
using Lunex.Application.Identity.Results;
using Lunex.Domain.Identity.Repositories;
using Lunex.Domain.Identity.ValueObjects;

using MediatR;

namespace Lunex.Application.Identity.Handlers;

public sealed class GetUserByEmailQueryHandler(
    IUserRepository userRepository) : IRequestHandler<GetUserByEmailQuery, UserResult?>
{
    public async Task<UserResult?> Handle(GetUserByEmailQuery query, CancellationToken cancellationToken)
    {
        var email = Email.Create(query.Email);
        var user = await userRepository.GetByEmailAsync(email, cancellationToken);
        return user is not null ? UserResult.FromDomain(user) : null;
    }
}