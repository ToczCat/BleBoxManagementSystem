using BBMS.Defaults;
using BBMS.Defaults.Identity;
using BBMS.Defaults.Models;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace BBMS.Gateway.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IOptions<SharedConfig> sharedConfig, ILogger<AuthController> logger) : ControllerBase
{
    private readonly GrpcChannel _identityChannel = GrpcChannel.ForAddress(
        $"{sharedConfig.Value.DefaultServiceGrpcScheme}://" +
        $"{sharedConfig.Value.IdentityServiceName}:" +
        $"{sharedConfig.Value.DefaultServiceGrpcPort}");

    [AllowAnonymous]
    [HttpGet("check")]
    public async Task<IActionResult> Check()
    {
        var client = new Auth.AuthClient(_identityChannel);
        var reply = await client.CheckAsync(new CheckRequest());

        return Ok(new { reply.IsSystemFresh });
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(UserDto request)
    {
        try
        {
            var client = new Auth.AuthClient(_identityChannel);
            var reply = await client.RegisterAsync(
                              new GuestRequest { Username = request.Username, Password = request.Password });

            return Ok(new User { Username = reply.Username });
        }
        catch (Exception ex)
        {
            logger.LogWarning("Failed to proceed with Register request, ex: {ex}", ex);
            return BadRequest();
        }
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<User>> Login(UserDto request)
    {
        var client = new Auth.AuthClient(_identityChannel);
        var reply = await client.LoginAsync(
                          new GuestRequest { Username = request.Username, Password = request.Password });

        return Ok(new User { Username = reply.Username, Token = reply.Token });
    }

    [Authorize]
    [RequiresClaim(ClaimTypes.Role, AuthenticationRole.User)]
    [HttpPut("update")]
    public async Task<ActionResult<User>> Update(string username, string password, AuthenticationRole role)
    {
        var client = new Auth.AuthClient(_identityChannel);
        var reply = await client.UpdateAsync(
                          new UpdateRequest { Username = username, Password = password, Role = role.ToString() });

        return Ok(new User { Username = reply.Username, Token = reply.Token });
    }

    [Authorize]
    [RequiresClaim(ClaimTypes.Role, AuthenticationRole.Admin)]
    [HttpDelete("delete")]
    public async Task<IActionResult> Delete(string username)
    {
        var client = new Auth.AuthClient(_identityChannel);
        var reply = await client.DeleteAsync(
                          new UserRequest { Username = username });

        return Ok();
    }
}
