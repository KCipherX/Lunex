using Lunex.Application.Common.Services.Abstractions;
using Lunex.Application.Member.Commands;
using Lunex.Application.Member.Results;
using Lunex.Domain.Member.Repositories;
using Lunex.Domain.Member.ValueObjects;

using MediatR;

using DomainMember = Lunex.Domain.Member.Aggregates.Member;

namespace Lunex.Application.Member.Handlers;

public sealed class CreateMemberProfileCommandHandler(
    IMemberRepository memberRepository,
    IDateTimeProvider dateTimeProvider) : IRequestHandler<CreateMemberProfileCommand, MemberResult>
{
    public async Task<MemberResult> Handle(CreateMemberProfileCommand command, CancellationToken cancellationToken)
    {
        var exists = await memberRepository.ExistsByUserIdAsync(command.UserId, cancellationToken);
        if (exists)
        {
            throw new InvalidOperationException("Member profile already exists for this user");
        }

        var gender = GenderExtensions.Parse(command.Gender);
        var dateOfBirth = DateOfBirth.Create(command.DateOfBirth);
        var location = Location.Create(command.City, command.Country);

        var member = DomainMember.CreateProfile(
            command.UserId,
            command.Name,
            gender,
            dateOfBirth,
            location,
            command.Description);

        await memberRepository.AddAsync(member, cancellationToken);

        return MemberResult.FromDomain(member);
    }
}