namespace AZDOI.Tests.Unit.Markdown;

public class TestMarkdownServiceTests
{
    [Fact]
    public async Task WriteIndex_ShouldWriteExpectedMarkdownContent()
    {
        // Given
        var (fileSystem, service) = ServiceProviderFixture.GetRequiredService<FakeFileSystem, TestMarkdownService>();
        string markdownContent = "# MyOrg DevOps Organization";

        // When
        var result = await service.TestWriteIndex(markdownContent);

        // Then
        await Verify(
            new
            {
                Content = result,
                Equal = markdownContent == result,
                FileExists = fileSystem.GetFile("/index.md").Exists
            }
            );
    }
}