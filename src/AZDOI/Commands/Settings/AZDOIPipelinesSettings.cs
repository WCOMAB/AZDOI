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

    [Description("Include specific releases")]
    [CommandOption("--include-release")]
    public override string[]? IncludeReleases { get; set; }

    [Description("Exclude specific releases")]
    [CommandOption("--exclude-release")]
    public override string[]? ExcludeReleases { get; set; }
}