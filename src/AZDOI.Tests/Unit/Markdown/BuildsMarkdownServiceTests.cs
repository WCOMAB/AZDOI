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
                        Url = "https://myproject.com" 
                    },
                    new AzureDevOpsPipeline 
                    { 
                        Id = 2, 
                        Name = "MyPipeline.Two", 
                        Url = "https://myproject.com" 
                    }
                ]
            };

            //When
            var result = await service.TestWriteIndex(project);

            //Then
            await Verify(result);
        }
}