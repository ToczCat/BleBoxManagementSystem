namespace BBMS.Defaults.Models;

public class SharedConfig
{
    public required string BackendServiceName { get; set; }
    public required string GatewayServiceName { get; set; }
    public required string IdentityServiceName { get; set; }
    public required string InterfaceServiceName { get; set; }
    public required string StorageServiceName { get; set; }
    public required string DashboardConnectionString { get; set; }
}