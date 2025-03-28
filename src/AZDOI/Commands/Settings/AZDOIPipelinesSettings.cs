using System.ComponentModel;

namespace AZDOI.Commands.Settings;

public class AZDOIPipelinesSettings(ICakeEnvironment environment)
    : AZDOISettings(environment, AzureDevOpsProjectChildTypes.Pipelines)
{
    [Description("Include specific pipelines")]
    [CommandOption("--include-pipeline")]
    public override string[]? IncludePipelines{ get; set; }

    [Description("Exclude specific pipelines")]
    [CommandOption("--exclude-pipeline")]
    public override string[]? ExcludePipelines { get; set; }
}