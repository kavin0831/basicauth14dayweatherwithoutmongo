using BlazorCookieAuthentication.Models;
using Microsoft.AspNetCore.Authorization;

namespace BlazorCookieAuthentication.Services;

public class CustomAuthorizeAttribute : AuthorizeAttribute
{
    public CustomAuthorizeAttribute(UserRole roleEnum)
    {
        Roles = roleEnum.ToString().Replace(" ", string.Empty);
    }
}
