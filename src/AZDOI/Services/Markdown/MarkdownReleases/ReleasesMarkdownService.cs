namespace AZDOI.Services.Markdown.Releases;

public class ReleasesMarkdownService(ICakeContext cakeContext, TimeProvider timeProvider)
    : MarkdownServiceBase<AzureDevOpsRelease[]>(cakeContext, timeProvider)
{
    protected override string? Title => "Releases";
    protected override string? Summary => "Azure DevOps Build Releases";

    protected override async Task WriteIndex(FileTextWriter writer, AzureDevOpsRelease[] children)
    {
        await WriteChildren(
           writer,
           children,
           "Release",
           "Releases",
           headingLevel: 1
        );
    }
}
