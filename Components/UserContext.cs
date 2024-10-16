using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Isopoh.Cryptography.Argon2;
using BlazorCookieAuthentication.Models;
using BlazorCookieAuthentication.Interfaces;
using BlazorCookieAuthentication.Services;

namespace BlazorCookieAuthentication.Components;

public class UserContext : ComponentBase
{
    [Inject] AuthenticationStateProvider Asp {get; set;} = default!;

    [Inject] IJSRuntime JSRuntime {get; set;} = default!;

    [Inject] IDatabase Database { get; set; } = default!;

    [Inject] NavigationManager Nav {get; set;}  = default!;

    private AuthenticationProvider Auth {get; set;} = default!;

    public UserInfo User => Auth.User;

    // Initlizer Function
    protected override void OnInitialized()
    {
        Auth = (AuthenticationProvider)Asp;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (!Auth.IsAuthenticated)
            {   
                await UserReAuthorize();
            }
        }

        if (Auth.IsAuthenticated)
        {
            await OnAfterRenderContextAsync(firstRender);
        }
    }

    // In your page override this method to be sure user is authenticated
    public virtual Task OnAfterRenderContextAsync(bool firstRender)
    {
        return Task.CompletedTask;
    }


    // Cookie Writer
    public async Task WriteCookie(string cookieName, string cookieValue, int durationMinutes = 1)
    {
        await JSRuntime.InvokeVoidAsync("CookieWriter.Write", cookieName, cookieValue, DateTime.Now.AddMinutes(durationMinutes));
    }

    // Cookie Reader
    public async Task<string> ReadCookie(string cookieName)
    {
        return await JSRuntime.InvokeAsync<string>("CookieReader.Read", cookieName);
    }

    // Cookie Remover
    public async Task<string> DeleteCookie(string cookieName)
    {
        string message = await JSRuntime.InvokeAsync<string>("CookieReader.Read", cookieName);
        await JSRuntime.InvokeVoidAsync("CookieRemover.Delete", cookieName);
        return message;
    }


    public async Task<bool> Login(string username, string password)
    {
        // Check if the input is empty
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) { return false; }

        // Grab the user from the Database
        var user = await Database.GetUser(username);

        // check if the user is Valid
        if (user.ID == Guid.Empty) { return false; }
        if (!Argon2.Verify(user.HashedPassword, password)) { return false; }

        var token = Database.CreateSessionToken(user);
        await WriteCookie("SessionID", token, 720);

        Auth.SetUser(user);
        return true;
    }

    public async Task Logout()
    {
        var token = await DeleteCookie("SessionID");
        Database.RemoveSessionToken(token);
        Auth.SetUser(new());
    }

    public async Task<bool> UserReAuthorize()
    {
        string token = await ReadCookie("SessionID");
        if (string.IsNullOrEmpty(token)) { return false;}
        
        var user = await Database.GetUserFromToken(token);
        if (user.ID == Guid.Empty) {return false; }

        Auth.SetUser(user);
        return true;
    }

    public void NavTo(string path, bool refresh = false)
    {
        Nav.NavigateTo(path, refresh);
    }

}