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

        if (!organization.SkipOrgGraph)
        {
            await WriteMermaidDiagram(writer, organization);
        }
    }
    public async Task WriteMermaidDiagram(FileTextWriter writer, AzureDevOpsOrganization organization)
    {
        await writer.WriteLineAsync("```mermaid");
        await writer.WriteLineAsync("graph TD");
        await writer.WriteLineAsync($"    Org_{organization.Id}({organization.Name})");
        await writer.WriteLineAsync();

        foreach (var project in organization.Children)
        {
            await writer.WriteLineAsync($"    %% {project.Name} project");
            await writer.WriteLineAsync($"    subgraph Proj_{project.Id}[{project.Name}]");
            await writer.WriteLineAsync("        direction TB");

            if (project.ChildTypes.HasFlag(AzureDevOpsProjectChildTypes.Repositories))
            {
                await writer.WriteLineAsync($"        %% {project.Name} repos");
                await writer.WriteLineAsync($"        subgraph Repos_{project.Id}[Repositories]");
                foreach (var repo in project.Children)
                {
                    var nodeId = $"Repo_{project.Id}_{repo.Id}";
                    var relativeUrl = $"{project.Name}/Repositories/{repo.Name}/";
                    await writer.WriteLineAsync($"            {nodeId}[{repo.Name}]");
                    await writer.WriteLineAsync($"            click {nodeId} href \"{relativeUrl}\" \"{repo.Name}\"");
                }
                await writer.WriteLineAsync("        end");
            }

            if (project.ChildTypes.HasFlag(AzureDevOpsProjectChildTypes.Pipelines))
            {
                await writer.WriteLineAsync($"        %% {project.Name} pipelines");
                await writer.WriteLineAsync($"        subgraph Pipelines_{project.Id}[Pipelines]");

                foreach (var pipeline in project.Pipelines)
                {
                    var nodeId = $"Pipeline_{project.Id}_{pipeline.Id}";
                    var relativeUrl = $"{project.Name}/Build/Pipelines/{pipeline.Name}/";
                    await writer.WriteLineAsync($"            {nodeId}[{pipeline.Name}]");
                    await writer.WriteLineAsync($"            click {nodeId} href \"{relativeUrl}\" \"{pipeline.Name}\"");
                }
                await writer.WriteLineAsync("        end");
            }

            await writer.WriteLineAsync("    end");
            await writer.WriteLineAsync();
            await writer.WriteLineAsync($"    Org_{organization.Id} --> Proj_{project.Id}");
        }
        await writer.WriteLineAsync("```");
    }
}