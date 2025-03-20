namespace AZDOI.Services.AzureDevOps;

public partial class AzureDevOpsClient
{
    public async IAsyncEnumerable<AzureDevOpsRepositoryTag> GetTagsAsync(string organization, string projectId, string repositoryId)
    {
        var url = $"https://dev.azure.com/{Uri.EscapeDataString(organization)}/{Uri.EscapeDataString(projectId)}/_apis/git/repositories/{repositoryId}/refs?filter=tags&api-version=7.1-preview.1";
        var response = await GetFromJson<AzureDevOpsResponse<AzureDevOpsRepositoryTag>>(url);
        if (response?.Value is not null)
        {
            foreach (var tag in response.Value)
            {
                yield return tag;
            }
        }
    }
}
