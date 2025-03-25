namespace AZDOI.Services.AzureDevOps;

public partial class AzureDevOpsClient
{
    public async Task<AzureDevOpsPipeline[]> GetPipelines(
       string organization,
       string projectId,
       IEnumerable<string>? includeNames = null,
       IEnumerable<string>? excludeNames = null)
    {
        var url = $"https://dev.azure.com/{Uri.EscapeDataString(organization)}/{Uri.EscapeDataString(projectId)}/_apis/pipelines?api-version=7.1";

        var result = await GetFromJson<AzureDevOpsResponse<AzureDevOpsPipeline>>(url);

        var pipelines = result?.Value ?? [];

        return pipelines.FilterEnumerable(
            includeNames,
            excludeNames,
            p => p.Id.ToString());
    }
}