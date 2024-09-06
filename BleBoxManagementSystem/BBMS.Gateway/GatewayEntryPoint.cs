using BBMS.Defaults;
using BBMS.Defaults.Identity;
using BBMS.Defaults.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddDefaultConfigurations();
var sharedConfig = builder.Configuration.GetSection(nameof(SharedConfig)).Get<SharedConfig>() ?? throw new Exception();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(x =>
    {
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = sharedConfig.Issuer,
            ValidAudience = sharedConfig.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(sharedConfig.SecretiestToken)),
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(ClaimTypes.Role, p => p.RequireClaim(IdentityData.AdminRoleClaimName, "admin", "user", "guest"));
});

builder.AddServiceDefaults(sharedConfig.GatewayServiceName, sharedConfig.DashboardConnectionString);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization Bearer Token",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
