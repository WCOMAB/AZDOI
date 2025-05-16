using AZDOI.Services.Markdown.Repositories;

namespace AZDOI.Tests.Unit.Markdown.Repositories;

public class RepositoriesMarkdownServiceTests
{
    [Fact]
    public async Task WriteIndex_ShouldWriteExpectedMarkdownContent()
    {
        // Given
        var (fileSystem, service) = ServiceProviderFixture
            .GetRequiredService<FakeFileSystem, RepositoriesMarkdownService>();

        AzureDevOpsRepository[] repositories = [
                new AzureDevOpsRepository
                {
                    Id = "1",
                    Name = "MyRepository.One",
                    Description = "MyRepository One Description",
                    Size = 100,
                    RemoteUrl = "https://myproject.com",
                    WebUrl = "https://myproject.com",
                    Url = "https://myproject.com"
                },
                new AzureDevOpsRepository
                {
                    Id = "2",
                    Name = "MyRepository.Two",
                    Description = "MyRepository Two Description",
                    Size = 100,
                    RemoteUrl = "https://myproject.com",
                    WebUrl = "https://myproject.com",
                    Url = "https://myproject.com"
                }
            ];

        // When
        var result = await service.TestWriteIndex(repositories);

        // Then
        await Verify(result);
    }
}