using System.ComponentModel;

namespace AZDOI.Commands.Settings;
public class AZDOIAllSettings(ICakeEnvironment environment)
    : AZDOISettings(
        environment,
        AzureDevOpsProjectChildTypes.Repositories
        | AzureDevOpsProjectChildTypes.Pipelines
    )
{
    [Description("Include specific repositories")]
    [CommandOption("--include-repository")]
    public override string[]? IncludeRepositories { get; set; }

    [Description("Exclude specific repositories")]
    [CommandOption("--exclude-repository")]
    public override string[]? ExcludeRepositories { get; set; }

    [Description("Include specific repository README")]
    [CommandOption("--include-repository-readme")]
    public override string[]? IncludeRepositoriesReadme { get; set; }

    [Description("Exclude specific repository README")]
    [CommandOption("--exclude-repository-readme")]
    public override string[]? ExcludeRepositoriesReadme { get; set; }

    [Description("Include specific pipelines")]
    [CommandOption("--include-pipeline")]
    public override string[]? IncludePipelines { get; set; }

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
