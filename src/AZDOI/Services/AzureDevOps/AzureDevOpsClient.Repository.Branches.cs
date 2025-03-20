namespace AZDOI.Services.AzureDevOps;

public partial class AzureDevOpsClient
{
    public async IAsyncEnumerable<AzureDevOpsRepositoryBranch> GetBranchesAsync(string organization, string projectId, string repositoryId)
    {
        var url = $"https://dev.azure.com/{Uri.EscapeDataString(organization)}/{Uri.EscapeDataString(projectId)}/_apis/git/repositories/{repositoryId}/refs?filter=heads&api-version=7.1-preview.1";

        var result = await GetFromJson<AzureDevOpsResponse<AzureDevOpsRepositoryBranch>>(url);

        if (result?.Value is { Length: > 0 } branches)
        {
            foreach (var branch in branches)
            {
                yield return branch;
            }
        }
    }
}