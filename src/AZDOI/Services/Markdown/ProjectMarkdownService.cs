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

        await WriteChildren(
            writer,
            project.Children,
            "Repository",
            "Repositories",
            urlSelector: child => $"Repositories/{child.Name}"
        );
    }
}