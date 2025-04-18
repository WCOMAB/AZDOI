[![Build Status](https://dev.azure.com/AZDOI/AZDOI/_apis/build/status%2FWCOMAB.AZDOI?repoName=WCOMAB%2FAZDOI&branchName=main)](https://dev.azure.com/AZDOI/AZDOI/_build/latest?definitionId=2&repoName=WCOMAB%2FAZDOI&branchName=main)
[![Build](https://github.com/WCOMAB/AZDOI/actions/workflows/build.yml/badge.svg)](https://github.com/WCOMAB/AZDOI/actions/workflows/build.yml)
[![NuGet Version](https://img.shields.io/nuget/v/azdoi)](https://www.nuget.org/packages/AZDOI/)
[![Contributor Covenant](https://img.shields.io/badge/Contributor%20Covenant-2.1-4baaaa.svg)](code_of_conduct.md)

# AZDOI

Azure DevOps Inventory .NET Tool – Inventories and documents an Azure DevOps organization by generating a set of Markdown files for the specified organization and saving them to a specified folder.

## Example Result

An example of the generated documentation can be viewed here: [AZDOI Example Documentation](https://wcomab.github.io/AZDOI/)

## Installation

### Global

```sh
dotnet tool install --global AZDOI
```

### Local

```sh
dotnet new tool-manifest
dotnet tool install --local AZDOI
```

## Authentication

Authentication via Azure DevOps can be done either with the use of a Person Access Token (PAT) or Azure Entra Id credentials.

### Azure Entra Id

By default it'll try authenticate using the [DefaultAzureCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.defaultazurecredential?view=azure-dotnet) which tries to authorize in the following order based on your environment.

1. [EnvironmentCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.environmentcredential?view=azure-dotnet)
1. [WorkloadIdentityCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.workloadidentitycredential?view=azure-dotnet)
1. [ManagedIdentityCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.managedidentitycredential?view=azure-dotnet)
1. [SharedTokenCacheCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.sharedtokencachecredential?view=azure-dotnet)
1. [VisualStudioCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.visualstudiocredential?view=azure-dotnet)
1. [VisualStudioCodeCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.visualstudiocodecredential?view=azure-dotnet)
1. [AzureCliCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.azureclicredential?view=azure-dotnet)
1. [AzurePowerShellCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.azurepowershellcredential?view=azure-dotnet)
1. [AzureDeveloperCliCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.azuredeveloperclicredential?view=azure-dotnet)
1. [InteractiveBrowserCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.interactivebrowsercredential?view=azure-dotnet)

## Commands

### Inventory repositories

#### Usage

```bash
azdoi inventory repositories <devopsorg> <outputpath> [OPTIONS]
```

#### Example

```bash
azdoi inventory repositories MyOrg /path/to/output
```

### Parameters

AZDOI uses the following environment variables for configuration:

| Parameter                      | Description                                 | Default Value                                   |
|--------------------------------|---------------------------------------------|-------------------------------------------------|
| `--help`                       | Used to get help with parameters            |                                                 |
| `--pat`                        | Personal Access Token for authentication    | Environment variable: `AZDOI_PAT`               |
| `--entra-id-auth`              | Use Entra Id for Azure Devops Autentication | False                                           |
| `--azure-tenant-id`            | Entra Azure Tenant ID for authentication    | Environment variable: `AZURE_TENANT_ID`         |
| `--run-in-parallel`            | Enable parallel processing of projects      | False                                           |
| `--include-project`            | Include specific projects                   |                                                 |
| `--exclude-project`            | Exclude specific projects                   |                                                 |
| `--include-repository`         | Include specific repositories               |                                                 |
| `--exclude-repository`         | Exclude specific repositories               |                                                 |
| `--include-repository-readme`  | Include specific repository README          |                                                 |
| `--exclude-repository-readme`  | Exclude specific repository README          |                                                 |
| `--skip-org-graph`             | Skip generating the org graph               |                                                 |

### Inventory pipelines

#### Usage

```bash
azdoi inventory pipelines <devopsorg> <outputpath> [OPTIONS]
```

#### Example

```bash
azdoi inventory pipelines MyOrg /path/to/output
```

### Parameters

AZDOI uses the following environment variables for configuration:

| Parameter                      | Description                                 | Default Value                                   |
|--------------------------------|---------------------------------------------|-------------------------------------------------|
| `--help`                       | Used to get help with parameters            |                                                 |
| `--pat`                        | Personal Access Token for authentication    | Environment variable: `AZDOI_PAT`               |
| `--entra-id-auth`              | Use Entra Id for Azure Devops Autentication | False                                           |
| `--azure-tenant-id`            | Entra Azure Tenant ID for authentication    | Environment variable: `AZURE_TENANT_ID`         |
| `--run-in-parallel`            | Enable parallel processing of projects      | False                                           |
| `--include-project`            | Include specific projects                   |                                                 |
| `--exclude-project`            | Exclude specific projects                   |                                                 |
| `--include-pipeline`           | Include specific pipeline                   |                                                 |
| `--exclude-pipeline`           | Exclude specific pipeline                   |                                                 |
| `--include-release`            | Include specific release                    |                                                 |
| `--exclude-release`            | Exclude specific release                    |                                                 |
| `--skip-org-graph`             | Skip generating the org graph               |                                                 |

### Inventory all

#### Usage

```bash
azdoi inventory all <devopsorg> <outputpath> [OPTIONS]
```

#### Example

```bash
azdoi inventory all MyOrg /path/to/output
```

### Parameters

AZDOI uses the following environment variables for configuration:

| Parameter                      | Description                                 | Default Value                                   |
|--------------------------------|---------------------------------------------|-------------------------------------------------|
| `--help`                       | Used to get help with parameters            |                                                 |
| `--pat`                        | Personal Access Token for authentication    | Environment variable: `AZDOI_PAT`               |
| `--entra-id-auth`              | Use Entra Id for Azure Devops Autentication | False                                           |
| `--azure-tenant-id`            | Entra Azure Tenant ID for authentication    | Environment variable: `AZURE_TENANT_ID`         |
| `--run-in-parallel`            | Enable parallel processing of projects      | False                                           |
| `--include-project`            | Include specific projects                   |                                                 |
| `--exclude-project`            | Exclude specific projects                   |                                                 |
| `--include-pipeline`           | Include specific pipeline                   |                                                 |
| `--exclude-pipeline`           | Exclude specific pipeline                   |                                                 |
| `--include-release`            | Include specific release                    |                                                 |
| `--exclude-release`            | Exclude specific release                    |                                                 |
| `--include-repository`         | Include specific repositories               |                                                 |
| `--exclude-repository`         | Exclude specific repositories               |                                                 |
| `--include-repository-readme`  | Include specific repository README          |                                                 |
| `--exclude-repository-readme`  | Exclude specific repository README          |                                                 |
| `--skip-org-graph`             | Skip generating the org graph               |                                                 |


## Setting environment variables

To set environment variables, use:

Windows (PowerShell)

```powershell
$env:AZDOI_PAT = "your_token_here"
$env:AZURE_TENANT_ID = "your_tenant_id_here"
```

Linux/macOS (Bash)

```sh
export AZDOI_PAT="your_token_here"
export AZURE_TENANT_ID="your_tenant_id_here"
```
