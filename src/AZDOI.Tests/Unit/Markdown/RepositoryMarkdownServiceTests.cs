namespace AZDOI.Tests.Unit.Markdown;

public class RepositoryMarkdownServiceTests
{
    [Fact]
    public async Task WriteIndex_ShouldWriteExpectedMarkdownContent()
    {
        // Given
        var (fileSystem, service) = ServiceProviderFixture
            .GetRequiredService<FakeFileSystem, RepositoryMarkdownService>();

        var repository = new AzureDevOpsRepository
        {
            Id = "1",
            Name = "MyRepository",
            Size = 2000000,
            RemoteUrl = "https://myproject.com",
            WebUrl = "https://myproject.com",
            ReadmeContent = """
                            # Example Readme

                            Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.

                            ## Getting started

                            Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.

                            ## Installation

                            Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.

                            ### Prerequisites

                            Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.
                            """,
            Url = "https://myproject.com",
            DefaultBranch = "refs/heads/main",
            Children = [
                new AzureDevOpsRepositoryBranch
                {
                    Name = "main",
                    ObjectId = "1573218",
                    Url = "https://mybranch.com"
                },
                new AzureDevOpsRepositoryBranch
                {
                    Name = "refs/heads/develop",
                    ObjectId = "12345",
                    Url = "https://helloworld.com"
                },
                new AzureDevOpsRepositoryBranch
                {
                    Name = "refs/heads/feature/450",
                    ObjectId = "54321",
                    Url = "https://Nvidia.com"
                }
            ],
            Tags = [
                new AzureDevOpsRepositoryTag
                {
                    Name = "refs/tags/v0.1",
                    ObjectId = "12345",
                    Url = "https://google.com"
                },
                new AzureDevOpsRepositoryTag
                {
                    Name = "refs/tags/2025.11.11",
                    ObjectId = "54321",
                    Url = "https://wcom.se"
                },
                new AzureDevOpsRepositoryTag
                {
                    Name = "refs/tags/2025.03.11",
                    ObjectId = "98765",
                    Url = "https://cloudflare.com",
                    Message = "Initial commit message"
                }
            ],
        };

        // When
        var result = await service.TestWriteIndex(repository);

        // Then
        await Verify(result);
    }

    [Theory]
    [InlineData("Readme Content")]
    [InlineData(null)]
    [InlineData("")]
    public async Task WriteIndex_ShouldWriteCorrectReadmeMarkdown(string? readmeContent)
    {
        // Given
        var (fileSystem, service) = ServiceProviderFixture
            .GetRequiredService<FakeFileSystem, RepositoryMarkdownService>();

        var repository = new AzureDevOpsRepository
        {
            Id = "1",
            Name = "MyRepository",
            Size = 2000000,
            RemoteUrl = "https://myproject.com",
            WebUrl = "https://myproject.com",
            ReadmeContent = readmeContent,
            Url = "https://myproject.com",
            DefaultBranch = "refs/heads/main",
            Children = [
                new AzureDevOpsRepositoryBranch
                {
                    Name = "main",
                    ObjectId = "1573218",
                    Url = "https://mybranch.com"
                }
            ],
            Tags = [
                new AzureDevOpsRepositoryTag
                {
                    Name = "refs/tags/v0.1",
                    ObjectId = "12345",
                    Url = "https://google.com"
                }
            ],
        };

        // When
        var result = await service.TestWriteIndex(repository);

        // Then
        await Verify(result);
    }
}
