﻿namespace AZDOI.Tests.Unit.Markdown;

public class PipelineMarkdownServiceTests
{
    [Fact]
    public async Task WriteIndex_ShouldWriteExpectedMarkdownContent()
    {
        // Given
        var (fileSystem, service) = ServiceProviderFixture
            .GetRequiredService<FakeFileSystem, PipelineMarkdownService>();

            var pipeline = 
            new AzureDevOpsPipeline
            {
                Id = 1,
                Name = "DevOps Pipeline",
                Url = "https://mypipeline.com",
            };
        
        // When
        var result = await service.TestWriteIndex(pipeline);

        // Then
        await Verify(result);
    }
}