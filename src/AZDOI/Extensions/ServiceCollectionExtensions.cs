using AZDOI.Services.AzureDevOps;
using AZDOI.Services.Markdown;
using Azure.Core;
using Azure.Identity;

namespace AZDOI.Extensions;

public static class ServiceCollectionExtensions
{
    private const string AzureDevOpsScope = "499b84ac-1321-427f-aa17-267ca6975798/.default";

    public static IServiceCollection AddAzureDevOpsClient(
        this IServiceCollection services,
        Func<IServiceProvider, IAzureDevOpsClientSettings, CancellationToken, Task<string>>? azureTokenProvider = null,
        Action<IHttpClientBuilder>? configure = default
        )
    {
        azureTokenProvider ??= GetDefaultTokenProvider();

        var httpClientBuilder = services.AddHttpClient<AzureDevOpsClientHandler>();
        configure?.Invoke(httpClientBuilder);

        services.AddTransient<AzureDevOpsClientHandler>(
           provider =>
           {
               var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();

               async Task<AzureDevOpsClient> GetAzureDevOpsClient(IAzureDevOpsClientSettings settings, CancellationToken cancellationToken = default)
               {
                   var httpClient = httpClientFactory.CreateClient(nameof(AzureDevOpsClientHandler));
                   if (settings.EntraIdAuth)
                   {
                       httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                                "Bearer",
                                await azureTokenProvider(provider, settings, cancellationToken)
                            );
                   }
                   else if (settings.Pat is { Length: > 0 } pat)
                   {
                       httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                            "Basic",
                            Convert.ToBase64String(Encoding.UTF8.GetBytes($"ado:{pat}"))
                        );
                   }

                   return new AzureDevOpsClient(httpClient);
               }

               return GetAzureDevOpsClient;
           }
           );
        return services;
    }

    private static Func<IServiceProvider, IAzureDevOpsClientSettings, CancellationToken, Task<string>> GetDefaultTokenProvider()
    {
        AccessToken? cachedToken = default;
        async Task<string> DefaultTokenProvider(IServiceProvider serviceprovider, IAzureDevOpsClientSettings settings, CancellationToken cancellationToken = default)
        {
            var logger = serviceprovider.GetRequiredService<ILogger<AzureDevOpsClientHandler>>();
            var credential = new DefaultAzureCredential(
                    string.IsNullOrWhiteSpace(settings.AzureTenantId)
                        ? new DefaultAzureCredentialOptions()
                        : new DefaultAzureCredentialOptions
                        {
                            TenantId = settings.AzureTenantId
                        }
                    );

            if (cachedToken is { } accessToken && accessToken.ExpiresOn > DateTimeOffset.UtcNow.AddMinutes(3))
            {
                logger.LogDebug("Cached token found, ExpiresOn: {ExpiresOn:yyyy-MM-dd HH:mm:ssz}.", accessToken.ExpiresOn);
                return accessToken.Token;
            }

            logger.LogInformation("Getting Azure token for Azure DevOps API...");

            var token = (
                            cachedToken = await credential.GetTokenAsync(
                                new TokenRequestContext(
                                    [
                                        AzureDevOpsScope
                                    ]
                                    ),
                                cancellationToken
                            )
                        ) ?? throw new InvalidOperationException("Failed to fetch acces token");

            logger.LogInformation("Got Azure token for Azure DevOps API, ExpiresOn: {ExpiresOn:yyyy-MM-dd HH:mm:ssz}.", token.ExpiresOn);

            return token.Token;
        }
        return DefaultTokenProvider;
    }

    public static IServiceCollection AddMarkdownServices(this IServiceCollection services)
    {
        services
            .AddSingleton<ProjectMarkdownService>()
            .AddSingleton<OrganizationMarkdownService>()
            .AddSingleton<RepositoriesMarkdownService>()
            .AddSingleton<RepositoryMarkdownService>()
            .AddSingleton<PipelineMarkdownService>()
            .AddSingleton<PipelinesMarkdownService>()
            .AddSingleton<BuildsMarkdownService>();

        return services;
    }
}

public delegate Task<AzureDevOpsClient> AzureDevOpsClientHandler(IAzureDevOpsClientSettings settings, CancellationToken cancellationToken = default);
