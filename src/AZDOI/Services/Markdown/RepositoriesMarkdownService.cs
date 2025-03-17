namespace AZDOI.Services.Markdown;

public class RepositoriesMarkdownService(ICakeContext cakeContext, TimeProvider timeProvider)
    : MarkdownServiceBase<AzureDevOpsRepository[]>(cakeContext, timeProvider)
{
    protected override async Task WriteIndex(FileTextWriter writer, AzureDevOpsRepository[] childeren)
    {
        await WriteChildren(
           writer,
           childeren,
           "Repository",
           "Repositories",
           headingLevel: 1
       );
    }
}