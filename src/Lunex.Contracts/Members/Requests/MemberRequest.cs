namespace Lunex.Contracts.Members.Requests;

public sealed record MemberRequest(
    string Name,
    string Email);

public sealed record MemberUpdateRequest(
    string DisplayName,
    string Description,
    string City,
    string Country);