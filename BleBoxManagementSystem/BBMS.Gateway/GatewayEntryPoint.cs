using BBMS.Defaults;
using BBMS.Defaults.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddDefaultConfigurations();
var sharedConfig = builder.Configuration.GetSection(nameof(SharedConfig)).Get<SharedConfig>();

builder.AddServiceDefaults(sharedConfig?.GatewayServiceName, sharedConfig?.DashboardConnectionString);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapDefaultEndpoints();

app.Run();
