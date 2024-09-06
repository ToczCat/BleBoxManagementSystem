namespace BBMS.Defaults.Models;

public class SharedConfig
{
    public required string BackendServiceName { get; set; }
    public required string GatewayServiceName { get; set; }
    public required string IdentityServiceName { get; set; }
    public required string InterfaceServiceName { get; set; }
    public required string StorageServiceName { get; set; }
    public required string DashboardConnectionString { get; set; }
    public required string DefaultServiceGrpcPort { get; set; }
    public required string DefaultServiceGrpcScheme { get; set; }
    public required string SecretiestToken { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
}