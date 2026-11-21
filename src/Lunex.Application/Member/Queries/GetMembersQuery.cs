using Lunex.Application.Member.Results;
using Lunex.Domain.Common.Pagination;

using MediatR;

namespace Lunex.Application.Member.Queries;

public sealed record GetMembersQuery(MemberParams Params) : IRequest<PaginatedResult<MemberResult>>;