namespace AZDOI.Services.AzureDevOps;

public partial class AzureDevOpsClient
{
    public async Task<AzureDevOpsProject[]> GetProjects(
        string organization,
        IEnumerable<string>? includeNames = null,
        IEnumerable<string>? excludeNames = null)
    {
        var url = $"https://dev.azure.com/{Uri.EscapeDataString(organization)}/_apis/projects?api-version=7.1-preview.1";

        var result = await GetFromJson<AzureDevOpsResponse<AzureDevOpsProject>>(url);

        var projects = result?.Value ?? [];

        return projects.FilterEnumerable(
          includeNames,
          excludeNames,
          p => p.Id
        );
    }
}
