using Lunex.Application.Member.Commands;
using Lunex.Application.Member.Results;
using Lunex.Domain.Member.Repositories;
using Lunex.Domain.Member.ValueObjects;

using MediatR;

namespace Lunex.Application.Member.Handlers;

public sealed class UpdateMemberProfileCommandHandler(
    IMemberRepository memberRepository) : IRequestHandler<UpdateMemberProfileCommand, MemberResult?>
{
    public async Task<MemberResult?> Handle(UpdateMemberProfileCommand command, CancellationToken cancellationToken)
    {
        var member = await memberRepository.GetByIdAsync(command.MemberId, cancellationToken);
        if (member is null)
        {
            return null;
        }

        Gender? gender = null;
        if (!string.IsNullOrEmpty(command.Gender))
        {
            gender = GenderExtensions.Parse(command.Gender);
        }

        DateOfBirth? dateOfBirth = null;
        if (command.DateOfBirth.HasValue)
        {
            dateOfBirth = DateOfBirth.Create(command.DateOfBirth.Value);
        }

        Location? location = null;
        if (!string.IsNullOrEmpty(command.City) && !string.IsNullOrEmpty(command.Country))
        {
            location = Location.Create(command.City, command.Country);
        }

        member.UpdateProfile(
            command.Name,
            gender,
            dateOfBirth,
            location,
            command.Description,
            command.ImageUrl);

        await memberRepository.UpdateAsync(member, cancellationToken);

        return MemberResult.FromDomain(member);
    }
}