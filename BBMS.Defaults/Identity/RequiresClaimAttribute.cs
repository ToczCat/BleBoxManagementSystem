using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BBMS.Defaults.Identity;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequiresClaimAttribute(string claimType, AuthenticationRole claimValue) : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        try
        {
            if (!(Enum.Parse<AuthenticationRole>(context.HttpContext
                    .User.FindFirst(c => c.Type == claimType)?.Value ?? AuthenticationRole.Guest.ToString()) <= claimValue))
                context.Result = new ForbidResult();
        }
        catch
        {
            context.Result = new ForbidResult();
        }
    }
}