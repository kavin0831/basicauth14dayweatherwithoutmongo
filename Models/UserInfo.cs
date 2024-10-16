using Microsoft.AspNetCore.Identity;

namespace BlazorCookieAuthentication.Models;

[Flags]
public enum UserRole
{
    Anonymous       = 1 << 0,
    Standard        = 1 << 1,
    Administrator   = 1 << 2,
}

public class UserInfo
{
    public Guid ID { get; set; } = Guid.Empty;

    public string Username { get; set; } = string.Empty;

    public string HashedPassword { get; set; } = string.Empty;

    public UserRole[] Roles { get; set; } = [ UserRole.Anonymous ];
}
