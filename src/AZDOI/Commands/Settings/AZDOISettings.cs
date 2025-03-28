using System.ComponentModel;
using AZDOI.Commands.Validation;

namespace AZDOI.Commands.Settings;

public abstract class AZDOISettings(
    ICakeEnvironment environment,
    AzureDevOpsProjectChildTypes projectChildTypes
    ) : CommandSettings, Services.AzureDevOps.IAzureDevOpsClientSettings
{
    public virtual AzureDevOpsProjectChildTypes ProjectChildTypes => projectChildTypes;

    [Description("Azure DevOps organization name")]
    [CommandArgument(0, "<devopsorg>")]
    [ValidateOrg]
    public required string DevOpsOrg { get; set; }

    [Description("Target directory for generated markdown files")]
    [CommandArgument(1, "<outputpath>")]
    [ValidatePath]
    public required DirectoryPath OutputPath { get; set; }

    [Description("Personal Access Token for Azure Devops Autentication")]
    [CommandOption("--pat")]
    public string? Pat { get; set; } = environment.GetEnvironmentVariable("AZDOI_PAT");

    [Description("Use Entra Id for Azure Devops Autentication")]
    [CommandOption("--entra-id-auth")]
    public bool EntraIdAuth { get; set; }

    [Description("Entra Azure Tenant Id for Azure Devops Autentication")]
    [CommandOption("--azure-tenant-id")]
    public string? AzureTenantId { get; set; } = environment.GetEnvironmentVariable("AZURE_TENANT_ID");

    [CommandOption("--run-in-parallel")]
    [Description("Flag for if generation should be parallelized.")]
    public bool RunInParallel { get; set; }

    [Description("Include specific projects")]
    [CommandOption("--include-project")]
    public string[]? IncludeProjects { get; set; }

    [Description("Exclude specific projects")]
    [CommandOption("--exclude-project")]
    public string[]? ExcludeProjects { get; set; }

    public virtual string[]? IncludeRepositories { get; set; }
           
    public virtual string[]? ExcludeRepositories { get; set; }
           
    public virtual string[]? IncludeRepositoriesReadme { get; set; }
           
    public virtual string[]? ExcludeRepositoriesReadme { get; set; }

    public virtual string[]? IncludePipelines { get; set; }

    public virtual string[]? ExcludePipelines { get; set; }
}
