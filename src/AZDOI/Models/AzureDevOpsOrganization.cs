namespace AZDOI.Models;

public record AzureDevOpsOrganization : AzureDevOpsBase<AzureDevOpsProject>
{
    internal bool SkipOrgGraph { get; init; } = false;
}