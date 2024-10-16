using BlazorCookieAuthentication.Models;
using BlazorCookieAuthentication.Interfaces;
using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace BlazorCookieAuthentication.Services;

class TokenStorage
{
    public string Token { get; set; } = string.Empty;
    public Guid UserID { get; set; } = Guid.NewGuid();
    public DateTime ExpireTime { get; set; } = DateTime.Now;
}

class FakeDatabase : IDatabase
{
    private readonly List<UserInfo> users = [];

    private readonly List<TokenStorage> tokens = [];

    public FakeDatabase()
    {
        users.Add(new UserInfo() {
            ID = Guid.NewGuid(),
            Username = "Admin",
            HashedPassword = Argon2.Hash("123"),
            Roles = [UserRole.Administrator, UserRole.Standard]
        });

        users.Add(new UserInfo() {
            ID = Guid.NewGuid(),
            Username = "User1",
            HashedPassword = Argon2.Hash("123"),
            Roles = [ UserRole.Standard ]
        });
    }

    public Task<UserInfo> GetUser(string username)
    {
        return Task.FromResult(users.FirstOrDefault(x => x.Username == username) ?? new());
    }

    public Task<UserInfo> GetUser(Guid userid)
    {
        return Task.FromResult(users.FirstOrDefault(x => x.ID == userid) ?? new());
    }

    public bool AddUser(UserInfo userInfo)
    {
        users.Add(userInfo);
        return true;
    }

    public string CreateSessionToken(UserInfo user)
    {
        var g = Guid.NewGuid().ToString("N");
        tokens.Add(new TokenStorage() {
            Token = g,
            UserID = user.ID,
            ExpireTime = DateTime.Now.AddHours(12)
        });

        Console.WriteLine("> Tokens");
        foreach (var item in tokens)
        {
            Console.WriteLine(">> {0}", item.Token);
        }

        return g;
    }

    public Task<UserInfo> GetUserFromToken(string token)
    {
        var t = tokens.FirstOrDefault(x => x.Token == token) ?? new();

        Console.WriteLine("?? Token {0}", t.Token);

        if (t.UserID == Guid.Empty) {return Task.FromResult(new UserInfo());}
        if (t.ExpireTime <= DateTime.Now)
        {
            Console.WriteLine("Expired !");
            tokens.Remove(t);
            return Task.FromResult(new UserInfo());
        }

        return GetUser(t.UserID);
    }

    public void RemoveSessionToken(string token)
    {
        tokens.RemoveAll(x => x.Token == token);
    }
}