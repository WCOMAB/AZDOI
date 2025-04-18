using AZDOI.Services;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace AZDOI.Tests.Unit.Commands;
public class InventoryAllCommandTests
{
    [Theory]

    [InlineData(false, "inventory", "all", "test-org", "/output", "--pat=test-pat")]
    [InlineData(true, "inventory", "all", "test-org", "/output", "--pat=test-pat")]
    [InlineData(true, "inventory", "all", "test-entraid-org", "/output", "--entra-id-auth")]
    [InlineData(true, "inventory", "all", "test-org", "/output", "--pat=test-pat", "--include-pipeline=123")]
    [InlineData(true, "inventory", "all", "test-org", "/output", "--pat=test-pat", "--exclude-pipeline=123")]
    [InlineData(true, "inventory", "all", "test-org", "/output", "--pat=test-pat", "--include-release=123")]
    [InlineData(true, "inventory", "all", "test-org", "/output", "--pat=test-pat", "--exclude-release=123")]
    [InlineData(true, "inventory", "all", "test-org", "/output", "--pat=test-pat", "--include-project=123")]
    [InlineData(true, "inventory", "all", "test-org", "/output", "--pat=test-pat", "--exclude-project=123")]
    [InlineData(true, "inventory", "all", "test-org", "/output", "--pat=test-pat", "--include-repository=456")]
    [InlineData(true, "inventory", "all", "test-org", "/output", "--pat=test-pat", "--exclude-repository=456")]
    [InlineData(true, "inventory", "all", "test-org", "/output", "--pat=test-pat", "--include-repository-readme=Test Repository")]
    [InlineData(true, "inventory", "all", "test-org", "/output", "--pat=test-pat", "--exclude-repository-readme=Test Repository")]
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
        var result = await commandApp.RunAsync(args);

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
