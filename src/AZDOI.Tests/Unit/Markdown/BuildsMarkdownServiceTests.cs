namespace AZDOI.Tests.Unit.Markdown;

public class BuildsMarkdownServiceTests
{
    [Fact]
        public async Task WriteIndex_ShouldWriteExpectedMarkdownContent()
        {
            //Given 
            var (fileSystem, service) = ServiceProviderFixture
                .GetRequiredService<FakeFileSystem, BuildsMarkdownService>();

            var project = new AzureDevOpsProject
            {
                Name = "Project",
                Id = "1",
                Url = "https://myproject.com",
                Pipelines =
                [
                    new AzureDevOpsPipeline 
                    {
                        Id = 1, 
                        Name = "MyPipeline.One",
                        Folder = "\\",
                        Revision = 3,
                        Links = new (
                            new ("https://myproject.com"),
                            new ("https://myproject.com")
                            )
                    },
                    new AzureDevOpsPipeline 
                    { 
                        Id = 2, 
                        Name = "MyPipeline.Two",
                        Folder = "\\",
                        Revision = 6,
                        Links = new (
                            new ("https://myproject.com"),
                            new ("https://myproject.com")
                            )
                    }
                ]
            };

            //When
            var result = await service.TestWriteIndex(project);

            //Then
            await Verify(result);
        }
}