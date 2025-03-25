namespace AZDOI.Services.Markdown;

public class BuildsMarkdownService(ICakeContext cakeContext, TimeProvider timeProvider)
    : MarkdownServiceBase<AzureDevOpsProject>(cakeContext, timeProvider)
{
    protected override async Task WriteIndex(FileTextWriter writer, AzureDevOpsProject project)
    {
        await writer.WriteLineAsync(
            $$"""
            ## Build

            """
        );

        await WriteChildren(
            writer,
            project.Pipelines,
            "Pipeline",
            "Pipelines",
            urlSelector: pipeline => $"Pipelines/{pipeline.Name}"
            );
    }
}