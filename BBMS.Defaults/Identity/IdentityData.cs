namespace BBMS.Defaults.Identity;

public static class IdentityData
{
    public const string AdminRoleClaimName = "admin";
    public const string UserRoleClaimName = "user";
    public const string GuestRoleClaimName = "guest";
}

public enum AuthenticationRole
{
    Admin,
    User,
    Guest
}