---
title: Getting started
---

## Setup Repository

Before you can use the Azure DevOps Inventory (AZDOI) tool in CI/CD pipelines, you need to set up your repository with the tool. Follow these steps to get started:

### 1. Create Repository

First, create a new repository to store your AZDOI configuration and reports:

```bash
# Create a new directory for your project
mkdir AzureDevOpsDocs
cd AzureDevOpsDocs

# Initialize a Git repository
git init
```

### 2. Create .NET Tool Manifest

The .NET tool manifest enables you to track the tools used in your project and ensure consistent tool versions across environments:

```bash
# Create a new .NET tool manifest
dotnet new tool-manifest
```

This command creates a `.config/dotnet-tools.json` file that will store information about the tools used in your project.

### 3. Install AZDOI Tool

Install the AZDOI tool and register it in the manifest:

```bash
# Install AZDOI tool
dotnet tool install AZDOI
```

This adds the AZDOI tool to your project's tool manifest, allowing it to be restored and used in CI/CD pipelines.

### 4. Commit Tool Manifest

Add the tool manifest to source control to ensure consistent tool versions:

```bash
# Add the tool manifest to Git
git add .config/dotnet-tools.json
git commit -m "Add AZDOI tool manifest"

# Push to your remote repository (optional)
# git remote add origin <your-repo-url>
# git push -u origin main
```

Now your repository is set up with the AZDOI tool and ready for configuring CI/CD pipelines.

## Azure Pipelines

Below is a fairly minimal Azure Pipeline that runs daily to generate an inventory of Azure DevOps repositories. This pipeline uses Azure CLI authentication and uploads the results as a pipeline artifact.


```yaml
# azure-pipelines.yml
trigger:
  - main

schedules:
- cron: "0 22 * * *"
  displayName: "Daily build at 22:00"
  branches:
    include:
      - main
  always: true

pool:
  vmImage: 'ubuntu-latest'

steps:
- task: AzureCLI@2
  displayName: 'Generate Azure DevOps Inventory'
  inputs:
    azureSubscription: 'azure-devops-inventory-tool'
    scriptType: 'bash'
    scriptLocation: 'inlineScript'
    inlineScript: |
      URL="$(System.CollectionUri)"
      ORG_NAME=${URL#https://dev.azure.com/}
      ORG_NAME=${ORG_NAME%/}
      echo "Organization name is: $ORG_NAME"

      dotnet tool restore \
        && dotnet AZDOI inventory repositories $ORG_NAME "$(Build.ArtifactStagingDirectory)" --entra-id-auth --run-in-parallel \
        && echo '##vso[artifact.upload artifactname=AzureDevOpsDocs]$(Build.ArtifactStagingDirectory)'
```

The Azure Pipeline configuration above does the following:

### Trigger
- Pipeline runs automatically when changes are pushed to the `main` branch
- Scheduled to run daily at 22:00 (10 PM) UTC through a cron schedule
- The schedule is set to always run, even if there are no code changes

### Environment
- Uses Ubuntu latest version as the build agent

### Steps
The pipeline has a single step using the Azure CLI task that:

1. Extracts the Azure DevOps organization name from the collection URI
2. Runs the AZDOI tool to generate an inventory of repositories by:
   - Restoring .NET tools
   - Running the `inventory repositories` command for the organization
   - Using Entra ID (formerly Azure AD) authentication
   - Running operations in parallel for better performance
   - Saving output to the build artifacts directory

3. Uploads the generated documentation as a build artifact named 'AzureDevOpsDocs'

The pipeline requires an Azure service connection named 'azure-devops-inventory-tool' with appropriate permissions to access Azure DevOps resources.

## GitHub Actions

Below is a GitHub Actions workflow that accomplishes the same goals as the Azure Pipeline mentioned. This workflow runs daily, generates an inventory of Azure DevOps repositories using Entra ID authentication, and uploads the results as artifacts.

```yaml
# .github/workflows/build.yml
name: Azure DevOps Repository Inventory

on:
  # Run daily at 10 PM UTC
  schedule:
    - cron: '0 22 * * *'
  # Allow manual triggering
  workflow_dispatch:
  # Optional: Run on pushes to main branch
  push:
    branches:
      - main

jobs:
  generate-inventory:
    name: Generate Repository Inventory
    runs-on: ubuntu-latest
    
    steps:
      - name: Azure Login
        uses: azure/login@v2
        with:
          creds: ${{ secrets.AZURE_CREDENTIALS }}
          
      - name: Checkout code
        uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '9.0.x'
      
      - name: Generate Azure DevOps Inventory
        env:
            ORG_NAME: ${{ secrets.AZURE_DEVOPS_ORG }}
        run: |
          mkdir -p "$GITHUB_WORKSPACE/output"
          
          # Install tools and run inventory
          dotnet tool restore
          dotnet AZDOI inventory repositories $ORG_NAME "$GITHUB_WORKSPACE/output" --entra-id-auth --run-in-parallel
      
      - name: Upload inventory report
        uses: actions/upload-artifact@v4
        with:
          name: AzureDevOpsInventory
          path: ${{ github.workspace }}/output
```

The GitHub Actions workflow configuration above does the following:

### Triggers
- Scheduled to run daily at 22:00 (10 PM) UTC through a cron schedule
- Can be manually triggered using the workflow_dispatch event
- Automatically runs when changes are pushed to the `main` branch

### Environment
- Uses Ubuntu latest version as the runner

### Steps
The workflow includes several steps that:

1. Authenticates with Azure using the Azure Login action
2. Checks out the repository code
3. Sets up the .NET environment with version 9.0
4. Generates the Azure DevOps inventory by:
   - Extracting the organization name from the DevOps URL
   - Creating an output directory
   - Restoring .NET tools
   - Running the `inventory repositories` command with Entra ID authentication
   - Using parallel processing for better performance

5. Uploads the generated documentation as a GitHub artifact named 'AzureDevOpsInventory'

The workflow requires Azure credentials stored as a GitHub secret named 'AZURE_CREDENTIALS' with appropriate permissions to access Azure DevOps resources.
