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
        await writer.WriteLineAsync($"    Org_{organization.Id}({organization.Name.HtmlEncode()})");
        await writer.WriteLineAsync();

        foreach (var project in organization.Children)
        {
            await writer.WriteLineAsync($"    %% {project.Name} project");
            await writer.WriteLineAsync($"    subgraph Proj_{project.Id}[{project.Name.HtmlEncode()}]");
            await writer.WriteLineAsync("        direction TB");

            var visibleNodes = new[]
            {
                new { Condition = project.ChildTypes.HasFlag(AzureDevOpsProjectChildTypes.Repositories), Node = $"Repos_{project.Id}" },
                new { Condition = project.ChildTypes.HasFlag(AzureDevOpsProjectChildTypes.Pipelines), Node = $"Pipelines_{project.Id}" },
                new { Condition = project.ChildTypes.HasFlag(AzureDevOpsProjectChildTypes.Pipelines) && project.Releases != null && project.Releases.Length > 0, Node = $"Releases_{project.Id}" }
            }
            .Where(item => item.Condition)
            .Select(item => item.Node)
            .ToList();

            if (visibleNodes.Count > 0)
            {
                var combinedLabel = string.Join("~~~", visibleNodes);
                await writer.WriteLineAsync($"        {combinedLabel}");
            }

            if (project.ChildTypes.HasFlag(AzureDevOpsProjectChildTypes.Repositories))
            {
                await writer.WriteLineAsync($"        subgraph Repos_{project.Id}[Repositories]");
                foreach (var repo in project.Children)
                {
                    var nodeId = $"Repo_{project.Id}_{repo.Id}";
                    var relativeUrl = $"{project.Name}/Repositories/{repo.Name}/";
                    await writer.WriteLineAsync($"            {nodeId}[{repo.Name.HtmlEncode()}]");
                    await writer.WriteClickNode(nodeId, relativeUrl, repo.Name);
                }
                await writer.WriteLineAsync("        end");
            }

            if (project.ChildTypes.HasFlag(AzureDevOpsProjectChildTypes.Pipelines))
            {
                await writer.WriteLineAsync($"        subgraph Pipelines_{project.Id}[Pipelines]");
                foreach (var pipeline in project.Pipelines)
                {
                    var nodeId = $"Pipeline_{project.Id}_{pipeline.Id}";
                    var relativeUrl = $"{project.Name}/Build/Pipelines/{pipeline.Name}/";
                    await writer.WriteLineAsync($"            {nodeId}[{pipeline.Name.HtmlEncode()}]");
                    await writer.WriteClickNode(nodeId, relativeUrl, pipeline.Name);
                }
                await writer.WriteLineAsync("        end");

                if (project.Releases != null && project.Releases.Length > 0)
                {
                    await writer.WriteLineAsync($"        subgraph Releases_{project.Id}[Releases]");
                    foreach (var release in project.Releases)
                    {
                        var nodeId = $"Release_{project.Id}_{release.Id}";
                        var relativeUrl = $"{project.Name}/Build/Releases/{release.Name}/";
                        await writer.WriteLineAsync($"            {nodeId}[{release.Name.HtmlEncode()}]");
                        await writer.WriteClickNode(nodeId, relativeUrl, release.Name);
                    }
                    await writer.WriteLineAsync("        end");
                }
            }

            await writer.WriteLineAsync("    end");
            await writer.WriteLineAsync();
            await writer.WriteLineAsync($"    Org_{organization.Id} --> Proj_{project.Id}");
        }
        await writer.WriteLineAsync("```");
    }
}