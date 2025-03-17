namespace AZDOI.Tests.Unit.Markdown;

public class ProjectMarkdownServiceTests
{
    [Fact]
    public async Task WriteIndex_ShouldWriteExpectedMarkdownContent()
    {
        // Given
        var (fileSystem, service) = ServiceProviderFixture
            .GetRequiredService<FakeFileSystem, ProjectMarkdownService>();

        var project = new AzureDevOpsProject
        {
            Id = "1",
            Name = "DevOps Project",
            Description = "DevOps Project Description",
            Url = "https://myproject.com",
            Children =
            [
                new AzureDevOpsRepository
                {
                    Id = "1",
                    Name = "MyRepository",
                    Description = "MyRepository Description",
                    Size = 100,
                    RemoteUrl = "https://myproject.com",
                    WebUrl = "https://myproject.com",
                    Url = "https://myproject.com"
                }
            ]
        };

        // When
        var result = await service.TestWriteIndex(project);

        // Then
        await Verify(result);
    }
}
