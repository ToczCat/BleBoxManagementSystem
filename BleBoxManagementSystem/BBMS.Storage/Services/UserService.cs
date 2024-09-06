using BBMS.Defaults;
using BBMS.Defaults.Identity;
using BBMS.Storage.Data;
using Grpc.Core;

namespace BBMS.Storage.Services;

public class UserService(IUserDatabase userDatabase, ILogger<UserService> logger) : User.UserBase
{
    public override Task<UserCheckReply> CheckUsers(UserCheckRequest request, ServerCallContext context)
    {
        try
        {
            logger.LogDebug("Admins check request received");

            return Task.FromResult(new UserCheckReply
            {
                Success = true,
                IsSystemFresh = userDatabase.CheckForAdmin(),
                Message = "ok"
            });
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to check admins in system, ex:{ex}", ex);
            throw new RpcException(new Status(StatusCode.Internal, "Failed to check admins in system"));
        }
    }

    public override Task<UserCredReply> Register(UserRegisterRequest request, ServerCallContext context)
    {
        try
        {
            logger.LogDebug("User registration request received");

            var registeredUsername = userDatabase.Register(request.Username, request.HashedPassword, AuthenticationRole.User);

            return Task.FromResult(new UserCredReply
            {
                Success = true,
                Username = request.Username,
                Role = AuthenticationRole.User.ToString(),
                Message = "ok"
            });
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to register user in system, ex:{ex}", ex);
            throw new RpcException(new Status(StatusCode.Internal, "Failed to register user in system"));
        }
    }

    public override Task<UserLoginReply> Login(UserLoginRequest request, ServerCallContext context)
    {
        try
        {
            logger.LogDebug("Login request received");

            var hashedPassword = userDatabase.Login(request.Username);

            return Task.FromResult(new UserLoginReply
            {
                Success = true,
                Username = request.Username,
                HashedPassword = hashedPassword,
                Message = "ok"
            });
        }
        catch (Exception  ex)
        {
            logger.LogError("Failed to login user, ex:{ex}", ex);
            throw new RpcException(new Status(StatusCode.Internal, "Failed to login user"));
        }
    }

    public override Task<UserCredReply> Update(UserUpdateRequest request, ServerCallContext context)
    {
        try
        {
            logger.LogDebug("User update request received");

            var username = userDatabase.Update(request.Username, request.HashedPassword, Enum.Parse<AuthenticationRole>(request.Role));

            return Task.FromResult(new UserCredReply
            {
                Success = true,
                Username = username,
                Message = "ok"
            });
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to update user, ex:{ex}", ex);
            throw new RpcException(new Status(StatusCode.Internal, "Failed to update user"));
        }
    }

    public override Task<UserDeleteReply> Delete(UserActionRequest request, ServerCallContext context)
    {
        try
        {
            logger.LogDebug("User delete request received");

            userDatabase.Delete(request.Username);

            return Task.FromResult(new UserDeleteReply
            {
                Success = true,
                Message = "ok"
            });
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to delete user, ex:{ex}", ex);
            throw new RpcException(new Status(StatusCode.Internal, "Failed to delete user"));
        }
    }
}
