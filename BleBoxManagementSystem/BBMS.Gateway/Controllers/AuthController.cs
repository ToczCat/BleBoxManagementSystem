using BBMS.Defaults;
using BBMS.Defaults.Models;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BBMS.Gateway.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IOptions<SharedConfig> sharedConfig) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(UserDto request)
    {
        using var channel = GrpcChannel.ForAddress($"http://{sharedConfig.Value.IdentityServiceName}:8080");
        var client = new Auth.AuthClient(channel);
        var reply = await client.RegisterAsync(
                          new UserRequest { Username = request.Username,  Password = request.Password });

        return Ok(new User { Username = reply.Username, Token = reply.Token});
    }

    [Authorize]
    [HttpPost("login")]
    public async Task<ActionResult<User>> Login(UserDto request)
    {
        using var channel = GrpcChannel.ForAddress($"http://{sharedConfig.Value.IdentityServiceName}:8080");
        var client = new Auth.AuthClient(channel);
        var reply = await client.LoginAsync(
                          new UserRequest { Username = request.Username, Password = request.Password });

        return Ok(new User { Username = reply.Username, Token = reply.Token });
    }
}
