namespace AZDOI.Commands;

public partial class InventoryCommand<TSettings>
{
    private async Task<AzureDevOpsPipeline[]> ProcessPipelines(InventoryContext context, AzureDevOpsProject project)
    {
        if (!context.Settings.ProjectChildTypes.HasFlag(AzureDevOpsProjectChildTypes.Pipelines))
        {
            return [];
        }

        Logger.LogInformation("Project: {ProjectName}", project.Name);

        var pipelinesDirectory = context.OutputDirectory.Combine("Pipelines");

        var pipelines =
            await context.InvokeDevOpsClient(
                async (client, settings) =>
                    await context.ForEachAsync(
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
        await services.PipelinesMarkdownService.WriteIndex(pipelinesDirectory, pipelines);

        return pipelines;
    }

    private async Task<AzureDevOpsPipeline> ProcessPipeline(InventoryContext context, AzureDevOpsProject project, AzureDevOpsPipeline sourcePipe)
    {
        using (Logger.BeginScope(new { Pipeline = sourcePipe.Id }))
        {
            Logger.LogInformation("Project: {ProjectName} - Pipeline: {PipelineName}", project.Name, sourcePipe.Name);

            await services.PipelineMarkdownService.WriteIndex(
                context.OutputDirectory.CombineEscapeUri(sourcePipe.Name),
                sourcePipe
            );

            Logger.LogInformation("Project: {ProjectName} - Pipeline: {PipelineName} - Markdown index created.", project.Name, sourcePipe.Name);

            Logger.LogInformation(
                "Project: {ProjectName} - Pipeline: {PipelineName}.",
                project.Name,
                sourcePipe.Name
            );
        }
        return sourcePipe;
    }
}