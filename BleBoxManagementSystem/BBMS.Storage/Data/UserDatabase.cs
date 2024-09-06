using BBMS.Defaults.Identity;
using BBMS.Defaults.Models;

namespace BBMS.Storage.Data;

public interface IUserDatabase
{
    bool CheckForAdmin();
    void Delete(string username);
    string Login(string username);
    string Register(string username, string hashedPassword, AuthenticationRole role);
    string Update(string username, string hashedPassword, AuthenticationRole role);
}

public class UserDatabase(ILogger<UserDatabase> logger) : IUserDatabase
{
    private List<SystemUser> _users = new List<SystemUser>
    {
        new SystemUser
        {
            Username = "Admin",
            HashedPassword = "$2a$11$dXE/fgzmmx6N.KZepMOk3.w1Vqvr6ECS5nge7ZVBOA5jIskapKOqi",
            Role = AuthenticationRole.Admin,
            Id = Guid.NewGuid()
        }
    };

    public bool CheckForAdmin()
    {
        logger.LogDebug("Checking if the system contains admin user");
        var isSystemFresh = !_users.Any(u => u.Role == AuthenticationRole.Admin);
        logger.LogDebug("System is{isfresh} fresh", isSystemFresh ? string.Empty : " not");

        return isSystemFresh;
    }

    public string Register(string username, string hashedPassword, AuthenticationRole role)
    {
        logger.LogDebug("Registering new user: {user} in database", username);
        if (_users.Any(u => u.Username == username))
        {
            logger.LogWarning("User: {user} already exists in database", username);
            throw new Exception("User already exists");
        }

        _users.Add(new SystemUser
        {
            Username = username,
            HashedPassword = hashedPassword,
            Role = role,
            Id = Guid.NewGuid()
        });

        logger.LogInformation("User: {user} registered successfully", username);

        return username;
    }

    public string Login(string username)
    {
        logger.LogDebug("Checking if user: {user} exists in database", username);

        if (!_users.Any(u => u.Username == username))
        {
            logger.LogWarning("User: {user} do not exist in database", username);
            throw new Exception("There is no such user");
        }

        var user = _users.First(u => u.Username == username);

        logger.LogInformation("User: {user} found", username);

        return user.HashedPassword;
    }

    public string Update(string username, string hashedPassword, AuthenticationRole role)
    {
        logger.LogDebug("Checking if user: {user} exists in database", username);

        if (!_users.Any(u => u.Username == username))
        {
            logger.LogWarning("User: {user} do not exist in database", username);
            throw new Exception("There is no such user");
        }

        var user = _users.First(u => u.Username == username);

        if (string.IsNullOrEmpty(hashedPassword))
            user.HashedPassword = hashedPassword;

        logger.LogInformation("User: {user} updated", username);

        return username;
    }

    public void Delete(string username)
    {
        logger.LogDebug("Checking if user: {user} exists in database", username);

        if (!_users.Any(u => u.Username == username))
        {
            logger.LogWarning("User: {user} do not exist in database", username);
            throw new Exception("There is no such user");
        }

        _users.RemoveAll(u => u.Username == username);

        logger.LogInformation("User: {user} deleted", username);
    }
}
