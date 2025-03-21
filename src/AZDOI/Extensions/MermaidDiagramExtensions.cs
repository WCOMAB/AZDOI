namespace AZDOI.Extensions;

public static class MermaidDiagramExtensions
{
    public static string GenerateMermaidDiagram(this AzureDevOpsOrganization organization)
    {
        var sb = new StringBuilder();

        sb.AppendLine("```mermaid");
        sb.AppendLine("graph TD");
        sb.AppendLine($"Org_{organization.Id}({organization.Name})");

        foreach (var project in organization.Children)
        {
            sb.AppendLine();
            sb.AppendLine($"%% {project.Name} project");
            sb.AppendLine($"subgraph Proj_{project.Id}[{project.Name}]");
            sb.AppendLine($"subgraph Repos_{project.Id}[Repositories]");
            foreach (var repo in project.Children)
            {
                sb.AppendLine($"Repo_{project.Id}_{repo.Id}[{repo.Name}]");
            }
            sb.AppendLine("end"); 
            sb.AppendLine("end"); 
            sb.AppendLine($"Org_{organization.Id} --> Proj_{project.Id}");
        }

        sb.AppendLine("```");

        return sb.ToString();
    }
}
