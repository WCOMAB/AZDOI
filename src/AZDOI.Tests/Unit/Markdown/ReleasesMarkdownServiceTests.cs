namespace AZDOI.Tests.Unit.Markdown;

public class ReleasesMarkdownServiceTests
{
    [Fact]
    public async Task WriteIndex_ShouldWriteExpectedMarkdownContent()
    {
        //Given 
        var (fileSystem, services) = ServiceProviderFixture
            .GetRequiredService<FakeFileSystem, ReleasesMarkdownService>();

        AzureDevOpsRelease[] release = [
            new AzureDevOpsRelease
            {
                Id = 1,
                Name = "MyRelease.One",
                Path = "\\",
                Revision = 2,
                Description = "This is a Release description",
                Links = new (
                    new ("https://myproject.com"),
                    new ("https://myproject.com")
                )
            },
            new AzureDevOpsRelease
            {
                Id = 2,
                Name = "MyRelease.Two",
                Path = "\\",
                Revision = 1,
                Links = new(
                    new("https://myproject.com"),
                    new("https://myproject.com")
                )
            },
            new AzureDevOpsRelease
            {
                Id = 5,
                Name = "MyRelease.Five",
                Path = "\\",
                Revision = 15,
                Description = "This is a Release description",
                Links = new(
                    new("https://myproject.com"),
                    new("https://myproject.com")
                )
            }
        ];

        //When
        var result = await services.TestWriteIndex(release);

        //Then
        await Verify(result);
    }
}