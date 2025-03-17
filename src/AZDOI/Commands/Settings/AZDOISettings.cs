using System.ComponentModel;
using AZDOI.Commands.Validation;

namespace AZDOI.Commands.Settings;

public class AZDOISettings(ICakeEnvironment environment) : CommandSettings, Services.AzureDevOps.IAzureDevOpsClientSettings
{
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

    [Description("Include specific projects")]
    [CommandOption("--include-project")]
    public string[]? IncludeProjects { get; set; }

    [Description("Exclude specific projects")]
    [CommandOption("--exclude-project")]
    public string[]? ExcludeProjects { get; set; }

    [Description("Include specific repositories")]
    [CommandOption("--include-repository")]
    public string[]? IncludeRepositories { get; set; }

    [Description("Exclude specific repositories")]
    [CommandOption("--exclude-repository")]
    public string[]? ExcludeRepositories { get; set; }

    [Description("Include specific repository README")]
    [CommandOption("--include-repository--readme")]
    public string[]? IncludeRepositoriesReadme { get; set; }

    [Description("Exclude specific repository README")]
    [CommandOption("--exclude-repository--readme")]
    public string[]? ExcludeRepositoriesReadme { get; set; }
}
