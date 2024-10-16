using BlazorCookieAuthentication.Models;

namespace BlazorCookieAuthentication.Interfaces;

public interface IDatabase
{
    public Task<UserInfo> GetUser(string username);

    public Task<UserInfo> GetUser(Guid userid);

    public bool AddUser(UserInfo userInfo);

    public string CreateSessionToken(UserInfo user);
    
    public void RemoveSessionToken(string token);

    public Task<UserInfo> GetUserFromToken(string token); 
}