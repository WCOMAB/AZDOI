#tool dotnet:?package=GitVersion.Tool&version=6.1.0
#load "build/records.cake"
#load "build/helpers.cake"
using System.Xml.Linq;
using System.Xml.XPath;

/*****************************
 * Setup
 *****************************/
Setup(
    static context => {
         var assertedVersions = context.GitVersion(new GitVersionSettings
            {
                OutputType = GitVersionOutput.Json
            });

        var branchName = assertedVersions.BranchName;
        var isMainBranch = StringComparer.OrdinalIgnoreCase.Equals("main", branchName);

        var gh = context.GitHubActions();
        var buildDate = DateTime.UtcNow;
        var runNumber = gh.IsRunningOnGitHubActions
                            ? gh.Environment.Workflow.RunNumber
                            : (short)((buildDate - buildDate.Date).TotalSeconds/3);

        var version = FormattableString
                    .Invariant($"{buildDate:yyyy.M.d}.{runNumber}");

        context.Information("Building version {0} (Branch: {1}, IsMain: {2})",
            version,
            branchName,
            isMainBranch);

        var artifactsPath = context
                            .MakeAbsolute(context.Directory("./artifacts"));

        var projectRoot =  context
                            .MakeAbsolute(context.Directory("./src"));

        var projectPath = projectRoot.CombineWithFilePath("AZDOI/AZDOI.csproj");

        return new BuildData(
            version,
            isMainBranch,
            !context.IsRunningOnLinux(),
            context.BuildSystem().IsLocalBuild,
            projectRoot,
            projectPath,
            new DotNetMSBuildSettings()
                .SetConfiguration("Release")
                .SetVersion(version)
                .WithProperty("Copyright", $"WCOM AB © {DateTime.UtcNow.Year}")
                .WithProperty("Authors", "wcomab")
                .WithProperty("Company", "wcomab")
                .WithProperty("PackageLicenseExpression", "MIT")
                .WithProperty("PackageTags", "tool;devops;docs;azure")
                .WithProperty("PackageDescription", "Azure DevOps Inventory .NET Tool – Inventories and documents an Azure DevOps organization by generating a set of Markdown files for the specified organization and saving them to a specified folder.")
                .WithProperty("RepositoryUrl", "https://github.com/WCOMAB/AZDOI.git")
                .WithProperty("ContinuousIntegrationBuild", gh.IsRunningOnGitHubActions ? "true" : "false")
                .WithProperty("EmbedUntrackedSources", "true"),
            artifactsPath,
            artifactsPath.Combine(version)
            );
    }
);

/*****************************
 * Tasks
 *****************************/
Task("Clean")
    .Does<BuildData>(
        static (context, data) => context.CleanDirectories(data.DirectoryPathsToClean)
    )
.Then("Restore")
    .Does<BuildData>(
        static (context, data) => context.DotNetRestore(
            data.ProjectRoot.FullPath,
            new DotNetRestoreSettings {
                MSBuildSettings = data.MSBuildSettings
            }
        )
    )
.Then("DPI")
    .Does<BuildData>(
        static (context, data) => context.DotNetTool(
                "tool",
                new DotNetToolSettings {
                    ArgumentCustomization = args => args
                                                        .Append("run")
                                                        .Append("dpi")
                                                        .Append("nuget")
                                                        .Append("--silent")
                                                        .AppendSwitchQuoted("--output", "table")
                                                        .Append(
                                                            (
                                                                !string.IsNullOrWhiteSpace(context.EnvironmentVariable("NuGetReportSettings_SharedKey"))
                                                                &&
                                                                !string.IsNullOrWhiteSpace(context.EnvironmentVariable("NuGetReportSettings_WorkspaceId"))
                                                            )
                                                                ? "report"
                                                                : "analyze"
                                                            )
                                                        .AppendSwitchQuoted("--buildversion", data.Version)
                }
            )
    )
.Then("Build")
    .DoesForEach<BuildData, FilePath>(
        static (data, context) => context.GetFiles(data.ProjectRoot.FullPath + "/**/*.csproj")
                                    .OrderBy(path => path.FullPath.EndsWith("AZDOI.csproj") ? 0 : 1)
                                    .ThenBy(path => path.FullPath, StringComparer.OrdinalIgnoreCase),
        static (data, item, context) => context.DotNetBuild(
            item.FullPath,
            new DotNetBuildSettings {
                NoRestore = true,
                MSBuildSettings = data.MSBuildSettings
            }
        )
    )
.Then("Test")
    .Does<BuildData>(
        static (context, data) => context.DotNetTest(
            data.ProjectRoot.FullPath,
            new DotNetTestSettings {
                NoBuild = true,
                NoRestore = true,
                MSBuildSettings = data.MSBuildSettings
            }
        )
    )
.Then("Pack")
    .Does<BuildData>(
        static (context, data) => context.DotNetPack(
            data.ProjectPath.FullPath,
            new DotNetPackSettings {
                NoBuild = true,
                NoRestore = true,
                OutputDirectory = data.NuGetOutputPath,
                MSBuildSettings = data.MSBuildSettings
            }
        )
    )
.Then("Upload-Artifacts")
    .WithCriteria(BuildSystem.IsRunningOnGitHubActions, nameof(BuildSystem.IsRunningOnGitHubActions))
    .Does<BuildData>(
        static (context, data) => context
            .GitHubActions() is var gh && gh != null
                ?   gh.Commands
                    .UploadArtifact(data.ArtifactsPath,  $"Artifact_{gh.Environment.Runner.ImageOS ?? gh.Environment.Runner.OS}_{context.Environment.Runtime.BuiltFramework.Identifier}_{context.Environment.Runtime.BuiltFramework.Version}")
                : throw new Exception("GitHubActions not available")
    )
    .Then("Integration-Tests-Tool-Manifest")
    .Does<BuildData>(
        static (context, data) => context.DotNetTool(
                "new",
                new DotNetToolSettings {
                    ArgumentCustomization = args => args
                                                        .Append("tool-manifest"),
                    WorkingDirectory = data.IntegrationTestPath
                }
            )
    )
.Then("Integration-Tests-Tool-Install")
    .Does<BuildData>(
        static (context, data) =>  context.DotNetTool(
                "tool",
                new DotNetToolSettings {
                    ArgumentCustomization = args => args
                                                        .Append("install")
                                                        .AppendSwitchQuoted("--add-source", data.NuGetOutputPath.FullPath)
                                                        .AppendSwitchQuoted("--version", data.Version)
                                                        .Append("AZDOI"),
                    WorkingDirectory = data.IntegrationTestPath
                }
            )
    )
.Then("Integration-Tests-Tool-Version")
    .Does<BuildData>(
        static (context, data) => context.DotNetTool(
                "tool",
                new DotNetToolSettings {
                    ArgumentCustomization = args => args
                                                        .Append("run")
                                                        .Append("--")
                                                        .Append("AZDOI")
                                                        .Append("--version"),
                    WorkingDirectory = data.IntegrationTestPath
                }
            )
    )
.Then("Integration-Tests-Tool")
    .WithCriteria<BuildData>((context, data) => data.ShouldRunIntegrationTests(), "ShouldRunIntegrationTests")
    .Does<BuildData>(
        static (context, data) => context.DotNetTool(
                "tool",
                new DotNetToolSettings {
                    ArgumentCustomization = args => args
                                                        .Append("run")
                                                        .Append("--")
                                                        .Append("AZDOI")
                                                        .Append("inventory")
                                                        .Append("repositories")
                                                        .Append("AZDOI")
                                                        .AppendQuoted(data.IntegrationTestPath.FullPath)
                                                        .Append("--entra-id-auth"),
                    WorkingDirectory = data.IntegrationTestPath
                }
            )
    )
.Then("Integration-Tests-Upload-Results")
    .WithCriteria(BuildSystem.IsRunningOnGitHubActions, nameof(BuildSystem.IsRunningOnGitHubActions))
    .WithCriteria<BuildData>((context, data) => data.ShouldRunIntegrationTests(), "ShouldRunIntegrationTests")
    .Does<BuildData>(
         async (context, data) => {
            var resultPath = data.IntegrationTestPath;
            await GitHubActions.Commands.UploadArtifact(
                resultPath,
                $"{data.AzureDomain}_{GitHubActions.Environment.Runner.ImageOS ?? GitHubActions.Environment.Runner.OS}_{context.Environment.Runtime.BuiltFramework.Identifier}_{context.Environment.Runtime.BuiltFramework.Version}"
            );
            GitHubActions.Commands.SetStepSummary(
                string.Join(
                    System.Environment.NewLine,
                    context.GetFiles($"{resultPath.FullPath}/**/*.md")
                        .Select(filePath => context.FileSystem.GetFile(filePath).ReadLines(Encoding.UTF8))
                        .SelectMany(line => line)
                )
            );
         }
    )
.Then("Integration-Tests")
.Default()
.Then("Generate-Static-Site-Install-PageFind")
    .Does(static context=>{
        FilePath PageFindPath = context.Tools.Resolve("pagefind") 
                                ??
                                context.Tools.Resolve("pagefind.cmd");
        
        if (PageFindPath == null)
        {
            context.Command(
                ["npm.cmd", "npm"],
                "install -g pagefind"
            );

            context.Command(
                ["pagefind", "pagefind.cmd"],
                "--version"
            );
        }
    })
.Then("Generate-Static-Site-Build")
    .Does<BuildData>(
        static (context, data) =>
    {
        var preview = context.Argument<bool>("preview", false);
        var port = context.Argument("port", "5080");
        context.EnsureDirectoryExists(data.StatiqWebOutputPath.FullPath);
        context.DotNetTool("wcomsite", new DotNetToolSettings
        {
            ArgumentCustomization = args => args
                .Append(preview ? "preview" : string.Empty)
                .AppendSwitchQuoted("-i", " ", "./src/site/theme")
                .AppendSwitchQuoted("-i", " ", "./src/site/AZDOI")
                .AppendSwitchQuoted("-i", " ", data.IntegrationTestPath.FullPath)
                .AppendSwitchQuoted("-o", " ", data.StatiqWebOutputPath.FullPath)
                .AppendSwitchQuoted("--port", " ", port)
                .AppendSwitchQuoted("--virtual-dir", " ", "/AZDOI"),
        });
    })
.Then("Generate-Static-Site-Index")
    .Does<BuildData>(
        static (context, data) =>
    {
        context.Command(
            ["pagefind", "pagefind.cmd"],
            new ProcessArgumentBuilder()
                .AppendSwitchQuoted("--site", data.StatiqWebOutputPath.FullPath)
        );
    }
)
.Then("Push-GitHub-Packages")
    .WithCriteria<BuildData>( (context, data) => data.ShouldPushGitHubPackages())
    .DoesForEach<BuildData, FilePath>(
        static (data, context)
            => context.GetFiles(data.NuGetOutputPath.FullPath + "/*.nupkg"),
        static (data, item, context)
            => context.DotNetNuGetPush(
                item.FullPath,
            new DotNetNuGetPushSettings
            {
                Source = data.GitHubNuGetSource,
                ApiKey = data.GitHubNuGetApiKey
            }
        )
    )
.Then("Push-NuGet-Packages")
    .WithCriteria<BuildData>( (context, data) => data.ShouldPushNuGetPackages())
    .DoesForEach<BuildData, FilePath>(
        static (data, context)
            => context.GetFiles(data.NuGetOutputPath.FullPath + "/*.nupkg"),
        static (data, item, context)
            => context.DotNetNuGetPush(
                item.FullPath,
                new DotNetNuGetPushSettings
                {
                    Source = data.NuGetSource,
                    ApiKey = data.NuGetApiKey
                }
        )
    )
.Then("Create-GitHub-Release")
    .WithCriteria<BuildData>( (context, data) => data.ShouldPushNuGetPackages())
    .Does<BuildData>(
        static (context, data) => context
            .Command(
                new CommandSettings {
                    ToolName = "GitHub CLI",
                    ToolExecutableNames = new []{ "gh.exe", "gh" },
                    EnvironmentVariables = { { "GH_TOKEN", data.GitHubNuGetApiKey } }
                },
                new ProcessArgumentBuilder()
                    .Append("release")
                    .Append("create")
                    .Append(data.Version)
                    .AppendSwitchQuoted("--title", data.Version)
                    .Append("--generate-notes")
                    .Append(string.Join(
                        ' ',
                        context
                            .GetFiles(data.NuGetOutputPath.FullPath + "/*.nupkg")
                            .Select(path => path.FullPath.Quote())
                        ))

            )
    )
.Then("GitHub-Actions")
.Run();
