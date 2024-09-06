using BBMS.Defaults;
using BBMS.Defaults.Models;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BBMS.Identity.Services;

public class AuthService(ILogger<AuthService> logger, IOptions<SharedConfig> sharedConfig) : Auth.AuthBase
{
    private readonly GrpcChannel _storageChannel = GrpcChannel.ForAddress(
        $"{sharedConfig.Value.DefaultServiceGrpcScheme}://" +
        $"{sharedConfig.Value.StorageServiceName}:" +
        $"{sharedConfig.Value.DefaultServiceGrpcPort}");

    public async override Task<CheckReply> Check(CheckRequest request, ServerCallContext context)
    {
        var client = new Defaults.User.UserClient(_storageChannel);
        var reply = await client.CheckUsersAsync(new UserCheckRequest());

        return new CheckReply
        {
            Success = reply.Success,
            IsSystemFresh = reply.IsSystemFresh,
            Message = reply.Message
        };
    }

    public async override Task<RegisterReply> Register(GuestRequest request, ServerCallContext context)
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var client = new Defaults.User.UserClient(_storageChannel);
        var reply = await client.RegisterAsync(new UserRegisterRequest
        {
            Username = request.Username,
            HashedPassword = hashedPassword
        });

        return new RegisterReply
        {
            Success = reply.Success,
            Username = reply.Username,
            Message = reply.Message
        };
    }

    public async override Task<LoginReply> Login(GuestRequest request, ServerCallContext context)
    {
        var client = new Defaults.User.UserClient(_storageChannel);
        var reply = await client.LoginAsync(new UserLoginRequest
        {
            Username = request.Username
        });

        var check = BCrypt.Net.BCrypt.Verify(request.Password, reply.HashedPassword);
        if (!reply.Success || !check)
            return new LoginReply
            {
                Success = false,
                Username = reply.Username,
                Token = string.Empty,
                Message = "Wrong username or password"
            };

        var token = CreateToken(request.Username, reply.Role);

        return new LoginReply
        {
            Success = reply.Success,
            Username = reply.Username,
            Token = token,
            Message = reply.Message
        };
    }

    public async override Task<LoginReply> Update(UpdateRequest request, ServerCallContext context)
    {
        var client = new Defaults.User.UserClient(_storageChannel);
        var reply = await client.UpdateAsync(new UserUpdateRequest
        {
            Username = request.Username,
            HashedPassword = string.IsNullOrEmpty(request.Password) ? BCrypt.Net.BCrypt.HashPassword(request.Password) : string.Empty,
            Role = request.Role,
        });

        return new LoginReply
        {
            Success = reply.Success,
            Username = reply.Username,
            Message = reply.Message
        };
    }

    public async override Task<DeleteReply> Delete(UserRequest request, ServerCallContext context)
    {
        var client = new Defaults.User.UserClient(_storageChannel);
        var reply = await client.DeleteAsync(new UserActionRequest
        {
            Username = request.Username
        });

        return new DeleteReply
        {
            Success = reply.Success,
            Message = reply.Message
        };
    }

    private string CreateToken(string username, string role)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role)
        };

        //must be minimum 128 bit
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(sharedConfig.Value.SecretiestToken));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds,
            issuer: sharedConfig.Value.Issuer,
            audience: sharedConfig.Value.Audience);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }
}
