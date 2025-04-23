using AZDOI.Services.Markdown.Releases;

namespace AZDOI.Tests.Unit.Markdown;

public class ReleaseMarkdownServiceTests
{
    [Fact]
    public async Task WriteIndex_ShouldWriteExpectedMarkdownContent()
    {
        // Given
        var (fileSystem, service) = ServiceProviderFixture
            .GetRequiredService<FakeFileSystem, ReleaseMarkdownService>();

        var release =
        new AzureDevOpsRelease
        {
            Id = 1,
            Name = "DevOps Release",
            Path = "\\",
            Revision = 3,
            Description = "Release Description",
            Links = new(
                new("https://myrelease.com"),
                new("https://myrelease.com")
            )
        };

        // When
        var result = await service.TestWriteIndex(release);

        // Then
        await Verify(result);
    }
}
