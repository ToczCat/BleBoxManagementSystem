using BBMS.Defaults;
using Grpc.Core;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BBMS.Identity.Services;
public class AuthService(ILogger<AuthService> logger) : Auth.AuthBase
{
    public override Task<LoginReply> Register(UserRequest request, ServerCallContext context)
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var token = CreateToken(request.Username, "Admin");

        return Task.FromResult(new LoginReply
        {
            Success = true,
            Username = request.Username,
            Token = token
        });
    }

    public override Task<LoginReply> Login(UserRequest request, ServerCallContext context)
    {
        var hashedPassword = "test"; //from db

        var check = BCrypt.Net.BCrypt.Verify(request.Password, hashedPassword);
        var token = CreateToken(request.Username, "Admin");

        return Task.FromResult(new LoginReply
        {
            Success = true,
            Username = request.Username,
            Token = token
        });
    }

    private string CreateToken(string username, string claim)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, claim)
        };

        //must be minimum 128 bit
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("secretiest key 83747823098324893209587923048920849320845920-75894073890578294037890758"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(claims: claims, expires: DateTime.Now.AddMinutes(30), signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }
}
