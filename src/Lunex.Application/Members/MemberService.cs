using Lunex.Application.Persistance.Interfaces;
using Lunex.Application.Services.Interfaces;
using Lunex.Domain.Enitities.Users;

namespace Lunex.Application.Members;

public sealed class MemberService(IMemberRepository memberRepository) : IMemberService
{
    public async Task<IReadOnlyList<User>> GetAsync(CancellationToken cancellationToken)
    {
        var members = await memberRepository.GetAsync(cancellationToken);
        return members;
    }

    public async Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var member = await memberRepository.GetByIdAsync(id, cancellationToken);
        return member;
    }
}
