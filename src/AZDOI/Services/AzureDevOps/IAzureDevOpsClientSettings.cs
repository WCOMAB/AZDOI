namespace AZDOI.Services.AzureDevOps;

public interface IAzureDevOpsClientSettings
{
    bool EntraIdAuth { get; }
    string? AzureTenantId { get; }
    string? Pat { get; }
}
