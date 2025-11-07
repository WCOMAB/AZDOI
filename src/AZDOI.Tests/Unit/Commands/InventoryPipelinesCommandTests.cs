using AZDOI.Services;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace AZDOI.Tests.Unit.Commands;
public class InventoryPipelinesCommandTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(false, "--help")]
    [InlineData(false, "inventory", "pipelines")]
    [InlineData(false, "inventory", "pipelines", "--help")]
    [InlineData(false, "inventory", "pipelines", "test-org")]
    [InlineData(false, "inventory", "pipelines", "test-org", "/output")]
    [InlineData(false, "inventory", "pipelines", "test-org", "/output", "--pat=test-pat")]
    [InlineData(false, "inventory", "pipelines", "test-org", "/output", "--pat=test-pat", "--include-pipeline=123")]
    [InlineData(false, "inventory", "pipelines", "test-org", "/output", "--pat=test-pat", "--exclude-pipeline=123")]
    [InlineData(false, "inventory", "pipelines", "test-org", "/output", "--pat=test-pat", "--include-project=123")]
    [InlineData(true, "inventory", "pipelines")]
    [InlineData(true, "inventory", "pipelines", "--help")]
    [InlineData(true, "inventory", "pipelines", "test-org")]
    [InlineData(true, "inventory", "pipelines", "test-org", "/output")]
    [InlineData(true, "inventory", "pipelines", "test-org", "/output", "--pat=test-pat")]
    [InlineData(true, "inventory", "pipelines", "test-org", "/output", "--pat=test-pat", "--include-pipeline=123")]
    [InlineData(true, "inventory", "pipelines", "test-org", "/output", "--pat=test-pat", "--exclude-pipeline=123")]
    [InlineData(true, "inventory", "pipelines", "test-org", "/output", "--pat=test-pat", "--include-project=123")]
    [InlineData(true, "inventory", "pipelines", "test-entraid-org", "/output", "--entra-id-auth", "--include-project=123")]
    [InlineData(true, "inventory", "pipelines", "test-org", "/output", "--pat=test-pat", "--include-release=123")]
    [InlineData(true, "inventory", "pipelines", "test-org", "/output", "--pat=test-pat", "--exclude-release=123")]
    public async Task RunAsync(bool outputPathExists, params string[] args)
    {
        // Given
        var (commandApp, testConsole, fakeFileSystem, fakeEnvironment, stopwatchProvider) = ServiceProviderFixture
                                           .GetRequiredService<ICommandApp, TestConsole, FakeFileSystem, FakeEnvironment, StopwatchProvider>(
                                               services => services.AuthorizedClient()
                                                                   .EntraIdAuthorizedClient()
                                           );
        fakeEnvironment.SetEnvironmentVariable("AZDOI_PAT", "test-pat");
        // When
        if (outputPathExists)
        {
            fakeFileSystem.CreateDirectory("/output");
        }
        Recording.Start();
        var result = await commandApp.RunAsync(args, TestContext.Current.CancellationToken);

        // Then
        await Verify(
                new
                {
                    ExitCode = result,
                    ConsoleOutput = testConsole.Output,
                    FileSystem = fakeFileSystem.FromDirectoryPath("/output")
                }
            );
    }
}
