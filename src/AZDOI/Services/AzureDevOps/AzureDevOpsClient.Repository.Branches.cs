namespace AZDOI.Services.AzureDevOps;

public partial class AzureDevOpsClient
{
    public async Task<AzureDevOpsRepositoryBranch[]> GetBranchesAsync(string organization, string projectId, string repositoryId)
    {
        var url = $"https://dev.azure.com/{Uri.EscapeDataString(organization)}/{Uri.EscapeDataString(projectId)}/_apis/git/repositories/{repositoryId}/refs?filter=heads&api-version=7.1-preview.1";

        var result = await GetFromJson<AzureDevOpsResponse<AzureDevOpsRepositoryBranch>>(url);

        var branches = result?.Value ?? [];

        return branches;
    }
}