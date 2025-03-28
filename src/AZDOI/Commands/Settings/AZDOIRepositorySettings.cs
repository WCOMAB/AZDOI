using System.ComponentModel;

namespace AZDOI.Commands.Settings;

public class AZDOIRepositorySettings(ICakeEnvironment environment)
    : AZDOISettings(environment, AzureDevOpsProjectChildTypes.Repositories)
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
}
