using BBMS.Defaults;
using BBMS.Defaults.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddDefaultConfigurations();
var sharedConfig = builder.Configuration.GetSection(nameof(SharedConfig)).Get<SharedConfig>();

builder.AddServiceDefaults(sharedConfig?.BackendServiceName, sharedConfig?.DashboardConnectionString);
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
//app.MapGrpcService<GreeterService>();
app.MapDefaultEndpoints();

app.Run();
