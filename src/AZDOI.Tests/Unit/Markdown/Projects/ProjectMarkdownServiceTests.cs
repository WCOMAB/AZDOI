﻿using AZDOI.Services.Markdown.Projects;

namespace AZDOI.Tests.Unit.Markdown;

public class ProjectMarkdownServiceTests
{
    private static readonly AzureDevOpsProject project = new()
    {
        Id = "1",
        Name = "DevOps Project",
        Description = "DevOps Project Description",
        Url = "https://myproject.com",
        Children =[
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
        ],
        Pipelines = [
            new AzureDevOpsPipeline {
                Id = 1,
                Name = "MyPipeline",
                Folder = "\\",
                Revision = 3,
                Links = new(
                    new("https://myproject.com/build/MyPipeline"),
                    new("https://myproject.com/build/MyPipeline")
                )
            }
        ],
        Releases = [
            new AzureDevOpsRelease
            {
                Id = 1,
                Name = "MyRelease.One",
                Path = "\\",
                Revision = 5,
                Description = "Release Description",
                Links = new (
                    new ("https://myproject.com"),
                    new ("https://myproject.com")
                )
            },
        ]
    };

    [Fact]
    public async Task WriteIndex_ShouldWriteExpectedMarkdownContent()
    {
        // Given
        var (fileSystem, service) = ServiceProviderFixture
            .GetRequiredService<FakeFileSystem, ProjectMarkdownService>();

        // When
        var result = await service.TestWriteIndex(project);

        // Then
        await Verify(result);
    }

    public class ChildTypes
    {

        [Theory]
        [InlineData(AzureDevOpsProjectChildTypes.None)]
        [InlineData(AzureDevOpsProjectChildTypes.Repositories)]
        [InlineData(AzureDevOpsProjectChildTypes.Pipelines)]
        [InlineData(AzureDevOpsProjectChildTypes.All)]
        public async Task WriteIndex_ShouldWriteExpectedMarkdownContent(AzureDevOpsProjectChildTypes childTypes)
        {
            // Given
            var (fileSystem, service) = ServiceProviderFixture
                .GetRequiredService<FakeFileSystem, ProjectMarkdownService>();

            var testProject = project with
            {
                ChildTypes = childTypes
            };

            // When
            var result = await service.TestWriteIndex(testProject);

            // Then
            await Verify(result);
        }
    }
}
