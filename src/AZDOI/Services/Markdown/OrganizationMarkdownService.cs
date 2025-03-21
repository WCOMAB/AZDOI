namespace AZDOI.Services.Markdown;

public class OrganizationMarkdownService(ICakeContext cakeContext, TimeProvider timeProvider)
 : MarkdownServiceBase<AzureDevOpsOrganization>(cakeContext, timeProvider)
{
    protected override async Task WriteIndex(FileTextWriter writer, AzureDevOpsOrganization organization)
    {
        await writer.WriteLineAsync(
            $$"""
            # {{organization.Name}} DevOps Organization

            """
        );

        await WriteChildren(writer, organization.Children, "Project");

        await WriteMermaidDiagram(writer, organization);

    }
    public async Task WriteMermaidDiagram(FileTextWriter writer, AzureDevOpsOrganization organization)
    {
        await writer.WriteLineAsync("```mermaid");
        await writer.WriteLineAsync("graph TD");
        await writer.WriteLineAsync($"    Org_{organization.Id}({organization.Name})");

        foreach (var project in organization.Children)
        {
            await writer.WriteLineAsync();
            await writer.WriteLineAsync($"    %% {project.Name} project");
            await writer.WriteLineAsync($"    subgraph Proj_{project.Id}[{project.Name}]");
            await writer.WriteLineAsync($"        subgraph Repos_{project.Id}[Repositories]");
            foreach (var repo in project.Children)
            {
                await writer.WriteLineAsync($"            Repo_{project.Id}_{repo.Id}[{repo.Name}]");
            }
            await writer.WriteLineAsync("        end");
            await writer.WriteLineAsync("    end");
            await writer.WriteLineAsync($"    Org_{organization.Id} --> Proj_{project.Id}");
        }

        await writer.WriteLineAsync("```");
    }
}