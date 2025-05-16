namespace AZDOI.Services.Markdown.Repositories;

public class RepositoriesMarkdownService(ICakeContext cakeContext, TimeProvider timeProvider)
    : MarkdownServiceBase<AzureDevOpsRepository[]>(cakeContext, timeProvider)
{
    protected override string? Title => "Repositories";
    protected override string? Summary => "Azure DevOps Repositories";

    protected override async Task WriteIndex(FileTextWriter writer, AzureDevOpsRepository[] children)
    {
        await WriteChildren(
           writer,
           children,
           "Repository",
           "Repositories",
           headingLevel: 1
        );
    }
}