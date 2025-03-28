namespace AZDOI.Commands;

public partial class InventoryPipelinesCommand
{
    private async Task<AzureDevOpsPipeline[]> ProcessPipelines(InventoryRepositoriesContext context, AzureDevOpsProject project)
    {
        logger.LogInformation("Project: {ProjectName}", project.Name);

        var pipelinesDirectory = context.OutputDirectory.Combine("Pipelines");

        var pipelines = await context.InvokeDevOpsClient(
                                async (client, settings) => await context.ForEachAsync(
                                                            await client.GetPipelines(settings.DevOpsOrg, project.Id, settings.IncludePipelines, settings.ExcludePipelines),
                                                        (pipeline, ct) => ProcessPipeline(
                                                                            context with
                                                                            {
                                                                                OutputDirectory = pipelinesDirectory,
                                                                            },
                                                                            project,
                                                                            pipeline
                                                        )
                                )
                                                        .OrderBy(_ => _.Name, StringComparer.OrdinalIgnoreCase)
                                                        .ToArrayAsync()
        );
        await pipelinesMarkdownService.WriteIndex(pipelinesDirectory, pipelines);

        return pipelines;
    }

    private async Task<AzureDevOpsPipeline> ProcessPipeline(InventoryRepositoriesContext context, AzureDevOpsProject project, AzureDevOpsPipeline sourcePipe)
    {
        using (logger.BeginScope(new { Pipeline = sourcePipe.Id }))
        {
            logger.LogInformation("Project: {ProjectName} - Pipeline: {PipelineName}", project.Name, sourcePipe.Name);


            await pipelineMarkdownService.WriteIndex(
                context.OutputDirectory.Combine(sourcePipe.Name),
                sourcePipe
            );

            logger.LogInformation("Project: {ProjectName} - Pipeline: {PipelineName} - Markdown index created.", project.Name, sourcePipe.Name);


            logger.LogInformation(
                "Project: {ProjectName} - Pipeline: {PipelineName}.",
                project.Name,
                sourcePipe.Name
            );
        }
        return sourcePipe;
    }
}