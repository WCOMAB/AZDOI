namespace AZDOI.Services.AzureDevOps;

public partial class AzureDevOpsClient
{
    public async Task<AzureDevOpsRepositoryAnnotatedTag?> GetAnnotatedTagsAsync(string organization, string projectId, string repositoryId, string objectId)
    {
        var url = $"https://dev.azure.com/{Uri.EscapeDataString(organization)}/{Uri.EscapeDataString(projectId)}/_apis/git/repositories/{repositoryId}/annotatedtags/{objectId}?api-version=7.1";

        var annotatedTag = await GetFromJson<AzureDevOpsRepositoryAnnotatedTag>(url);

        return annotatedTag;
    }
}
