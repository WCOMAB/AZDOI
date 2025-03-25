namespace AZDOI.Services.Markdown;

public class RepositoriesMarkdownService(ICakeContext cakeContext, TimeProvider timeProvider)
    : MarkdownServiceBase<AzureDevOpsRepository[]>(cakeContext, timeProvider)
{
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