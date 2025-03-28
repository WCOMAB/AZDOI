namespace AZDOI.Services.Markdown;

public class PipelinesMarkdownService(ICakeContext cakeContext, TimeProvider timeProvider)
    : MarkdownServiceBase<AzureDevOpsPipeline[]>(cakeContext, timeProvider)
{
    protected override string? Title => "Pipelines";
    protected override string? Summary => "Azure DevOps Build Pipelines";

    protected override async Task WriteIndex(FileTextWriter writer, AzureDevOpsPipeline[] children)
    {
        await WriteChildren(
           writer,
           children,
           "Pipeline",
           "Pipelines",
           headingLevel: 1
        );
    }
}