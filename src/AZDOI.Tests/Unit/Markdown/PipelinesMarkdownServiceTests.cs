using AZDOI.Services.Markdown.Builds.Pipelines;

namespace AZDOI.Tests.Unit.Markdown;

public class PipelinesMarkdownServiceTests
{
    [Fact]
    public async Task WriteIndex_ShouldWriteExpectedMarkdownContent()
    {
        // Given
        var (fileSystem, service) = ServiceProviderFixture
        .GetRequiredService<FakeFileSystem, PipelinesMarkdownService>();

        AzureDevOpsPipeline[] pipelines = [
            new AzureDevOpsPipeline
            {
                Id = 1,
                Name = "MyPipeline.One",
                Folder = "\\",
                Revision = 3,
                Links = new(
                            new("https://mypipeline.com"),
                            new("https://mypipeline.com")
                            )
            },
            new AzureDevOpsPipeline
            {
                Id = 2,
                Name = "MyPipeline.Two",
                Folder = "\\",
                Revision = 6,
                Links = new(
                            new("https://mypipeline.com"),
                            new("https://mypipeline.com")
                            )
            },
        ];

        //When
        var result = await service.TestWriteIndex(pipelines);

        //Then
        await Verify(result);
    }
}
