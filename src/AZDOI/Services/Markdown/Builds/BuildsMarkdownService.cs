namespace AZDOI.Services.Markdown.Builds;

public class BuildsMarkdownService(ICakeContext cakeContext, TimeProvider timeProvider)
    : MarkdownServiceBase<AzureDevOpsProject>(cakeContext, timeProvider)
{
    protected override string? Title => "Build";
    protected override string? Summary => "Azure DevOps Builds";

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

        await WriteChildren(
            writer,
            project.Releases,
            "Release",
            "Releases",
            urlSelector: release => $"Releases/{release.Name}"
            );
    }
}