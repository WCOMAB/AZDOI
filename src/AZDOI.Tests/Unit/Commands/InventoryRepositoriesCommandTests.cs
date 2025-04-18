using AZDOI.Services;
using Microsoft.Extensions.Logging.Testing;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace AZDOI.Tests.Unit.Commands;

public class InventoryRepositoriesCommandTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(false, "--help")]
    [InlineData(false, "inventory", "repositories")]
    [InlineData(false, "inventory", "repositories", "--help")]
    [InlineData(false, "inventory", "repositories", "test-org")]
    [InlineData(false, "inventory", "repositories", "test-org", "/output")]
    [InlineData(false, "inventory", "repositories", "test-org", "/output", "--pat=test-pat")]
    [InlineData(false, "inventory", "repositories", "test-org", "/output", "--pat=test-pat", "--include-project=123")]
    [InlineData(false, "inventory", "repositories", "test-org", "/output", "--pat=test-pat", "--include-project=123", "--include-repository-readme=Test Repository")]
    [InlineData(false, "inventory", "repositories", "test-org", "/output", "--pat=test-pat", "--include-project=123", "--exclude-repository-readme=Test Repository")]
    [InlineData(true, "inventory", "repositories")]
    [InlineData(true, "inventory", "repositories", "--help")]
    [InlineData(true, "inventory", "repositories", "test-org")]
    [InlineData(true, "inventory", "repositories", "test-org", "/output")]
    [InlineData(true, "inventory", "repositories", "test-org", "/output", "--pat=test-pat")]
    [InlineData(true, "inventory", "repositories", "test-org", "/output", "--pat=test-pat", "--include-project=123")]
    [InlineData(true, "inventory", "repositories", "test-org", "/output", "--pat=test-pat", "--include-project=123", "--include-repository-readme=Test Repository")]
    [InlineData(true, "inventory", "repositories", "test-org", "/output", "--pat=test-pat", "--include-project=123", "--exclude-repository-readme=Test Repository")]
    [InlineData(true, "inventory", "repositories", "test-entraid-org", "/output", "--entra-id-auth", "--include-project=123")]
    [InlineData(true, "inventory", "repositories", "test-entraid-org", "/output", "--entra-id-auth", "--include-project=123", "--include-repository-readme=Test Repository")]
    [InlineData(true, "inventory", "repositories", "test-entraid-org", "/output", "--entra-id-auth", "--include-project=123", "--exclude-repository-readme=Test Repository")]
    [InlineData(true, "inventory", "repositories", "test-org", "/output", "--pat=test-pat", "--skip-org-graph")]
    [InlineData(false, "inventory", "repositories", "test-org", "/output", "--pat=test-pat", "--skip-org-graph")]
    public async Task RunAsync(bool outputPathExists, params string[] args)
    {
        // Given
        var (commandApp, testConsole, fakeLog, fakeFileSystem, fakeEnvironment, stopwatchProvider) = ServiceProviderFixture
                                           .GetRequiredService<ICommandApp, TestConsole, FakeLogger<InventoryRepositoriesCommand>, FakeFileSystem, FakeEnvironment, StopwatchProvider>(
                                               services => services.AuthorizedClient()
                                                                   .EntraIdAuthorizedClient()
                                           );
        fakeEnvironment.SetEnvironmentVariable("AZDOI_PAT", "test-pat");
        // When
        if (outputPathExists)
        {
            fakeFileSystem.CreateDirectory("/output");
        }
        var result = await commandApp.RunAsync(args);

        // Then
        await Verify(
                new
                {
                    ExitCode = result,
                    ConsoleOutput = testConsole.Output,
                    LogOutput = fakeLog.Collector.GetSnapshot(),
                    FileSystem = fakeFileSystem.FromDirectoryPath("/output")
                }
            );
    }
}
