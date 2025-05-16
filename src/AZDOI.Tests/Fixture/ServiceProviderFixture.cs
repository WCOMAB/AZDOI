using AZDOI.Extensions;
using AZDOI.Services;
using AZDOI.Services.AzureDevOps;
using AZDOI.Tests;
using Devlead.Console.Extensions;
using Devlead.Testing.MockHttp;
using Microsoft.Extensions.Logging;
using Spectre.Console.Testing;
using VerifyTests.MicrosoftLogging;

public static partial class ServiceProviderFixture
{

    static partial void InitServiceProvider(IServiceCollection services)
    {
        services
            .AddCakeCoreFakes()
            .AddMockHttpClient<Constants>()
            .AddSingleton<AzureDevOpsClient>()
            .AddSingleton<TestMarkdownService>()
            .AddMarkdownServices()
            .AddSingleton<InventoryCommandServices>()
            .AddSingleton<FakeStopwatch>()
            .AddSingleton<StopwatchProvider>(provider => provider.GetRequiredService<FakeStopwatch>())
            .AddSingleton<ILoggerProvider, RecordingProvider>();

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
                { })
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