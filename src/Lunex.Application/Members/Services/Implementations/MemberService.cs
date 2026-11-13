using Lunex.Application.Members.Persistence.Abstractions;
using Lunex.Application.Members.Services.Abstractions;
using Lunex.Domain.Enitities.Members;
using Lunex.Domain.Enitities.Photos;

namespace Lunex.Application.Members.Services.Implementations;

public sealed class MemberService(IMemberRepository memberRepository) : IMemberService
{
    public async Task<IReadOnlyList<Member>> GetAsync(CancellationToken cancellationToken)
    {
        var members = await memberRepository.GetAsync(cancellationToken);
        return members;
    }

    public async Task<Member?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var member = await memberRepository.GetByIdAsync(id, cancellationToken);
        return member;
    }

    public async Task<IReadOnlyList<Photo>> GetPhotosAsync(string id, CancellationToken cancellationToken)
    {
        var memberPhotos = await memberRepository.GetPhotosAsync(id, cancellationToken);
        return memberPhotos;
    }

    public async Task UpdateAsync(Member member, CancellationToken cancellationToken)
    {
        await memberRepository.UpdateAsync(member, cancellationToken);
    }
}
