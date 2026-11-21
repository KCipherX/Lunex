using Lunex.Application.Identity.Queries;
using Lunex.Application.Identity.Results;
using Lunex.Domain.Identity.Repositories;

using MediatR;

namespace Lunex.Application.Identity.Handlers;

public sealed class GetUserByIdQueryHandler(
    IUserRepository userRepository) : IRequestHandler<GetUserByIdQuery, UserResult?>
{
    public async Task<UserResult?> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(query.UserId, cancellationToken);
        return user is not null ? UserResult.FromDomain(user) : null;
    }
}