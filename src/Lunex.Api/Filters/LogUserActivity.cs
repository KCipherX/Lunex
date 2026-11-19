using Lunex.Api.Extensions.Utilities;
using Lunex.Application.Members.Services.Abstractions;

using Microsoft.AspNetCore.Mvc.Filters;

namespace Lunex.Api.Filters;

public sealed class LogUserActivity(IMemberService memberService) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.HttpContext.User.Identity?.IsAuthenticated != true)
        {
            return;
        }

        var userId = context.HttpContext.User.GetUserId();
        if (!string.IsNullOrEmpty(userId))
        {
            await memberService.UpdateLastActiveAsync(userId, context.HttpContext.RequestAborted);
        }
    }
}
