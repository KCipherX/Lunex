using Lunex.Application.Identity.Results;

using MediatR;

namespace Lunex.Application.Identity.Queries;

public sealed record GetUserByIdQuery(string UserId) : IRequest<UserResult?>;