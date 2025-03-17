namespace AZDOI.Tests.Unit.Markdown;

public class OrganizationMarkdownServiceTests
{
    [Fact]
    public async Task WriteIndex_ShouldWriteExpectedMarkdownContent()
    {
        // Given
        var (fileSystem, service) = ServiceProviderFixture
            .GetRequiredService<FakeFileSystem, OrganizationMarkdownService>();

        var organization = new AzureDevOpsOrganization
        {
            Id = "1",
            Url = "",
            Name = "DevOps Organization",
            Children = [ new AzureDevOpsProject
            {
                Id = "1",
                Name = "MyProject",
                Description = "MyProject Description",
                Url = "https://myproject.com"
            } ]
        };

        // When
        var result = await service.TestWriteIndex(organization);

        // Then
        await Verify(result);

    }
}
