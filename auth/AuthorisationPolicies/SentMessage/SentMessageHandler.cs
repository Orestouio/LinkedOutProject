
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BackendApp.Service;
using Microsoft.AspNetCore.Authorization;

namespace BackendApp.auth.Filters;

public class SentMessageHandler
(IHttpContextAccessor httpContextAccessor, IRegularUserService userService, IMessageService messageService)
: AuthorizationHandler<SentMessageRequirement>
{
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
    private readonly IRegularUserService userService = userService;
    private readonly IMessageService messageService = messageService;

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        SentMessageRequirement requirement
    )
    {
        if(context.User.HasClaim(ClaimTypes.Role, "admin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
        
        var notificationIdString = httpContextAccessor.HttpContext?
            .GetRouteData()
            .Values[requirement.MessageIdParamName]
            ?.ToString();

        if( notificationIdString is null )
        {
            context.Fail();
            return Task.CompletedTask;
        }

        var userClaims = context.User.Claims;
        var userIdClaim = userClaims.FirstOrDefault( c => c.Type == ClaimTypes.Sid);
        if(userIdClaim is null)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        var user = this.userService.GetUserById(long.Parse(userIdClaim.Value));
        var message = this.messageService.GetMessageById(long.Parse(notificationIdString));

        if(message is not null && message.SentBy == user)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
        else
        {
            context.Fail();
            return Task.CompletedTask;
        }
    }
}