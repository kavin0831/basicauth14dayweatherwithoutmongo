using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Microsoft.JSInterop;

using BlazorCookieAuthentication.Models;
using BlazorCookieAuthentication.Interfaces;
using Isopoh.Cryptography.Argon2;

namespace BlazorCookieAuthentication.Services;

class AuthenticationProvider : AuthenticationStateProvider
{
    // User Information Goes Here !
    public UserInfo User  {get; private set; } = new();

    public bool IsAuthenticated => User.ID != Guid.Empty;

    // This sets the Authentication State with User Roles
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // Default Auth state == no Identity
        AuthenticationState auth = new(new ClaimsPrincipal(new ClaimsIdentity()));
        
        // if the user is Assinged, let set the roles
        if (IsAuthenticated){
            List<Claim> claims = [
                new Claim(ClaimTypes.Role, UserRole.Anonymous.ToString())
        ]   ;
            foreach (var role in User.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
            }
            claims.Add(new Claim(ClaimTypes.Name, User.Username));
            auth = new(new ClaimsPrincipal(new ClaimsIdentity(claims, "BlazorCookieAuthentication")));
        }

        return Task.FromResult(auth);
    }

    public void SetUser(UserInfo user)
    {
        User = user;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}