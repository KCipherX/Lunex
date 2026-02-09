using Lunex.Application.Members.Persistence.Abstractions;
using Lunex.Application.Members.Services.Abstractions;
using Lunex.Domain.Enitities.Users;

namespace Lunex.Application.Members.Services.Implementations;

public sealed class MemberService(IMemberRepository memberRepository) : IMemberService
{
    public async Task<IReadOnlyList<User>> GetAsync(CancellationToken cancellationToken)
    {
        var members = await memberRepository.GetAsync(cancellationToken);
        return members;
    }

    public async Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var member = await memberRepository.GetByIdAsync(id, cancellationToken);
        return member;
    }
}
