using Lunex.Application.Identity.Results;

using MediatR;

namespace Lunex.Application.Identity.Queries;

public sealed record GetUserByEmailQuery(string Email) : IRequest<UserResult?>;