namespace AZDOI.Services.AzureDevOps;

public partial class AzureDevOpsClient
{
    public async Task<AzureDevOpsRelease[]> GetReleases(
       string organization, 
       string projectId,
       IEnumerable<string>? includeNames = null,
       IEnumerable<string>? excludeNames = null)
    {
        var url = $"https://vsrm.dev.azure.com/{Uri.EscapeDataString(organization)}/{Uri.EscapeDataString(projectId)}/_apis/release/definitions?api-version=7.2-preview.4";

        var result = await GetFromJson<AzureDevOpsResponse<AzureDevOpsRelease>>(url);

        var releases = result?.Value ?? [];

        return releases.FilterEnumerable(
            includeNames,
            excludeNames,
            p => p.Id.ToString());
    }
}
