using AZDOI.Extensions;
using AZDOI.Services.AzureDevOps;
using AZDOI.Tests;
using Cake.Core;
using Cake.Core.Configuration;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Devlead.Console.Extensions;
using Devlead.Testing.MockHttp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using NSubstitute;
using Spectre.Console.Testing;

public static partial class ServiceProviderFixture
{
    public static IServiceCollection AddCakeFakes(
        this IServiceCollection services,
        Action<FakeFileSystem>? configureFileSystem = null)
    {
        var configuration = new FakeConfiguration();
        var environment = FakeEnvironment.CreateUnixEnvironment();
        var fileSystem = new FakeFileSystem(environment);
        configureFileSystem?.Invoke(fileSystem);
        var globber = new Globber(fileSystem, environment);
        var log = new FakeLog();

        // Create a fake ICakeContext using NSubstitute.
        var Context = Substitute.For<ICakeContext>();
        Context.Configuration.Returns(configuration);
        Context.Environment.Returns(environment);
        Context.FileSystem.Returns(fileSystem);
        Context.Globber.Returns(globber);
        Context.Log.Returns(log);

        return services.AddSingleton<ICakeConfiguration>(configuration)
                       .AddSingleton<FakeEnvironment>(environment)
                       .AddSingleton<ICakeEnvironment>(environment)
                       .AddSingleton<FakeFileSystem>(fileSystem)
                       .AddSingleton<IFileSystem>(fileSystem)
                       .AddSingleton<IGlobber>(globber)
                       .AddSingleton<ICakeLog>(log)
                       .AddSingleton<ICakeRuntime>(environment.Runtime)
                       .AddSingleton(Context);
    }

    static partial void InitServiceProvider(IServiceCollection services)
    {
        var logger = new FakeLogger<InventoryRepositoriesCommand>();

        services
            .AddCakeFakes()
            .AddMockHttpClient<Constants>()
            .AddSingleton<AzureDevOpsClient>()
            .AddSingleton<TestMarkdownService>()
            .AddMarkdownServices();

        services
            .AddSingleton(logger)
            .AddSingleton<ILogger<InventoryRepositoriesCommand>>(
               provider => provider.GetRequiredService<FakeLogger<InventoryRepositoriesCommand>>()
            );

        services
                .AddCommandApp(new TestConsole());
    }
    public static IServiceCollection AuthorizedClient(this IServiceCollection services)
    {
        return ConfigureMockHttpClient(services);
    }
    public static IServiceCollection EntraIdAuthorizedClient(this IServiceCollection services)
    {
        return services.ConfigureMockHttpClient<Constants>(
                client =>
                {})
            .AddAzureDevOpsClient((sp, settings, ct) => Task.FromResult("fake-entraid-token"));
    }
    private static IServiceCollection ConfigureMockHttpClient(IServiceCollection services)
    {
        return services.ConfigureMockHttpClient<Constants>(
            client =>
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation(
                    "Authorization",
                    "Basic YWRvOnRlc3QtcGF0"
                );
            }
        );
    }
}