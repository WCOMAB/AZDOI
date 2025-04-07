namespace AZDOI.Tests.Unit.Markdown;

public class OrganizationMarkdownServiceTests
{
    [Fact]
    public async Task WriteIndex_ShouldWriteExpectedMarkdownContent()
    {
        // Given
        var (fileSystem, service) = ServiceProviderFixture
            .GetRequiredService<FakeFileSystem, OrganizationMarkdownService>();

        var organization = new AzureDevOpsOrganization
        {
            Id = "1",
            Url = "",
            Name = "DevOps Organization",
            Children = [ new AzureDevOpsProject
            {
                Id = "1",
                Name = "MyProject",
                Description = "MyProject Description",
                Url = "https://myproject.com"
            } ]
        };

        // When
        var result = await service.TestWriteIndex(organization);

        // Then
        await Verify(result);
    }

    public class OrgMarkdownChildTypes
    {
        [Theory]
        [InlineData(AzureDevOpsProjectChildTypes.None)]
        [InlineData(AzureDevOpsProjectChildTypes.Repositories)]
        [InlineData(AzureDevOpsProjectChildTypes.Pipelines)]
        [InlineData(AzureDevOpsProjectChildTypes.All)]
        public async Task WriteIndex_ShouldWriteExpectedMarkdownContent(AzureDevOpsProjectChildTypes childTypes)
        {
            var (fileSystem, service) = ServiceProviderFixture
                .GetRequiredService<FakeFileSystem, OrganizationMarkdownService>();

            var organization = new AzureDevOpsOrganization
            {
                Id = "Org1",
                Name = "Test Organization",
                Url = "",
                Children =[
                    new AzureDevOpsProject
                    {
                        Id = "Proj1",
                        Name = "Test Project",
                        ChildTypes = childTypes,
                        Url = "https://myproject.com",
                        Children = childTypes.HasFlag(AzureDevOpsProjectChildTypes.Repositories)
                            ?
                            [
                                new AzureDevOpsRepository 
                                {
                                    Id = "1",
                                    Name = "MyRepository",
                                    Size = 2000000,
                                    RemoteUrl = "https://myproject.com",
                                    WebUrl = "https://myproject.com",
                                    Url = "https://myproject.com"
                                }
                            ]
                            : [],
                        Pipelines = childTypes.HasFlag(AzureDevOpsProjectChildTypes.Pipelines)
                            ?
                            [
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
                                }
                            ]
                            : []
                    }
                ]
            };

            var result = await service.TestWriteIndex(organization);

            await Verify(result);
        }

    }
}
