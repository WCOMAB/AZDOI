using System.Net.Http.Json;

namespace AZDOI.Services.AzureDevOps;

public partial class AzureDevOpsClient(HttpClient httpClient) : IDisposable
{
    public void Dispose()
    {
        using(httpClient) { }
        GC.SuppressFinalize(this);
    }

    private async Task<T?> GetFromJson<T>(string url)
    {
        var response = await httpClient.GetAsync(url);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return default;
        }
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<T>();
        return result;
    }
}
