using AZDOI.Services.AzureDevOps;

namespace AZDOI.Tests.Unit.Clients;

public class AzureDevOpsClientTests
{
    public class FilterProjects
    {
        [Theory]
        [InlineData("test-org", new string[0], new string[0])]
        [InlineData("test-org", new string[0], new string[] { "999" })]
        [InlineData("test-org", new string[] { "123" }, new string[0])]
        [InlineData("test-org", new string[] { "123", "321", "999" }, new string[] { "321" })]
        public async Task GetProjects(string organization, string[] includeNames, string[] excludeNames)
        {
            // Given
            var azureDevOpsClient = ServiceProviderFixture
                                        .GetRequiredService<AzureDevOpsClient>(services => services.AuthorizedClient());

            // When
            var result = await azureDevOpsClient.GetProjects(organization, includeNames, excludeNames);

            // Then
            await Verify(result);
        }
    }

    public class FilterRepositories
    {
        [Theory]
        [InlineData("test-org", "123", new string[0], new string[0])]
        [InlineData("test-org", "123", new string[0], new string[] { "666" })]
        [InlineData("test-org", "123", new string[] { "456" }, new string[0])]
        [InlineData("test-org", "123", new string[] { "456", "654", "666" }, new string[] { "654" })]
        public async Task GetRepositories(string organization, string project, string[] includeNames, string[] excludeNames)
        {
            // Given
            var azureDevOpsClient = ServiceProviderFixture
                                        .GetRequiredService<AzureDevOpsClient>(services => services.AuthorizedClient());

            // When
            var result = await azureDevOpsClient.GetRepositories(organization, project, includeNames, excludeNames);

            // Then
            await Verify(result);
        }
    }

    [Theory]
    [InlineData("test-org", "123", "456")]
    public async Task GetRepositoryReadme(string organization, string projectId, string repositoryId)
    {
        // Given
        var azureDevOpsClient = ServiceProviderFixture
                                    .GetRequiredService<AzureDevOpsClient>(services => services.AuthorizedClient());
        // When
        var result = await azureDevOpsClient.GetRepositoryReadme(organization, projectId, repositoryId);
        // Then
        await Verify(result);
    }

    [Theory]
    [InlineData("test-org", "123", "456")]
    public async Task GetRepositoryBranches(string organization, string projectId, string repositoryId)
    {
        // Given
        var azureDevOpsClient = ServiceProviderFixture
                                    .GetRequiredService<AzureDevOpsClient>(services => services.AuthorizedClient());
        // When
        var result = await azureDevOpsClient.GetBranchesAsync(organization, projectId, repositoryId)
                        .ToArrayAsync(cancellationToken: TestContext.Current.CancellationToken);
        // Then
        await Verify(result);
    }

    [Theory]
    [InlineData("test-org", "123", "456")]
    public async Task GetRepositoryTags(string organization, string projectId, string repositoryId)
    {
        // Given
        var azureDevOpsClient = ServiceProviderFixture
                                    .GetRequiredService<AzureDevOpsClient>(services => services.AuthorizedClient());

        // When
        var result = await azureDevOpsClient.GetTagsAsync(organization, projectId, repositoryId)
                        .ToListAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Then
        await Verify(result);
    }

    [Theory]
    [InlineData("test-org", "123", "456", "jdaosyn3u23jas82jssa8")]
    public async Task GetRepositoryAnnotatedTags(string organization, string projectId, string repositoryId, string objectId)
    {
        // Given
        var azureDevOpsClient = ServiceProviderFixture
                                    .GetRequiredService<AzureDevOpsClient>(services => services.AuthorizedClient());
        // When
        var result = await azureDevOpsClient.GetAnnotatedTagsAsync(organization, projectId, repositoryId, objectId);
        // Then
        await Verify(result);
    }
}
