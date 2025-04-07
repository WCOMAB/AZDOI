namespace AZDOI.Services.Markdown;

public class ProjectMarkdownService(ICakeContext cakeContext, TimeProvider timeProvider)
    : MarkdownServiceBase<AzureDevOpsProject>(cakeContext, timeProvider)
{
    protected override async Task WriteIndex(FileTextWriter writer, AzureDevOpsProject project)
    {
        await writer.WriteLineAsync(
            $$"""
            # {{project.Name}}
            
            ## Project Details

            """
        );

        await WriteTable(
            writer,
            [
                GetKeyValue(project.Name),
                GetKeyValue(project.Description),
                GetKeyValue(project.Id),
            ]
            );

        if (project.ChildTypes.HasFlag(AzureDevOpsProjectChildTypes.Repositories))
        {
            await WriteChildren(
                writer,
                project.Children,
                "Repository",
                "Repositories",
                urlSelector: child => $"Repositories/{child.Name}"
            );
        }

        if (project.ChildTypes.HasFlag(AzureDevOpsProjectChildTypes.Pipelines))
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
                urlSelector: pipeline => $"Build/Pipelines/{pipeline.Name}"
            );

            await WriteChildren(
                writer,
                project.Releases,
                "Release",
                "Releases",
                urlSelector: release => $"Build/Releases/{release.Name}"
            );
        }
    }
}
