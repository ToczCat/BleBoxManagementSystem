using BBMS.Defaults.Identity;

namespace BBMS.Defaults.Models;

public class SystemUser
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string HashedPassword { get; set; } = string.Empty;
    public AuthenticationRole Role { get; set; } = AuthenticationRole.Guest;
}
