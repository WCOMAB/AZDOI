namespace AZDOI.Services.AzureDevOps;

public partial class AzureDevOpsClient
{
    public async Task<AzureDevOpsRepository[]> GetRepositories(
        string organization,
        string projectId,
        IEnumerable<string>? includeNames = null,
        IEnumerable<string>? excludeNames = null)
    {
        var url = $"https://dev.azure.com/{Uri.EscapeDataString(organization)}/{Uri.EscapeDataString(projectId)}/_apis/git/repositories?api-version=7.1-preview.1";

        var result = await GetFromJson<AzureDevOpsResponse<AzureDevOpsRepository>>(url);

        var projects = result?.Value ?? [];

        return projects.FilterEnumerable(
            includeNames,
            excludeNames,
            p => p.Id
        );
    }
}
