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

        var mermaid = organization.GenerateMermaidDiagram(); 

        await writer.WriteLineAsync(mermaid);

    }
}