using Microsoft.Extensions.Logging.Testing;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace AZDOI.Tests.Unit.Commands;

public class ValidateOrgAttributeTest
{
    [Theory]
    [InlineData("inventory", "repositories", "test-org", "/output")]
    [InlineData("inventory", "repositories", "한자-org", "/output")]
    [InlineData("inventory", "repositories", "!test-org", "/output")]
    [InlineData("inventory", "repositories", "test-org!", "/output")]
    [InlineData("inventory", "repositories", "test-orggggggggggggggggggggggggggggggggggggggggggggg", "/output")]

    public async Task RunAsync(params string[] args)
    {
        // Given
        var (commandApp, testConsole, fakeLog, fakeFileSystem) = ServiceProviderFixture
                                           .GetRequiredService<ICommandApp, TestConsole, FakeLogger<InventoryRepositoriesCommand>, FakeFileSystem>(
                                               services => services.AuthorizedClient()
                                                                   .EntraIdAuthorizedClient()
                                           );

        // When
        fakeFileSystem.CreateDirectory("/output");
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
            )
            .DontIgnoreEmptyCollections()
            .AddExtraSettings(setting => setting.DefaultValueHandling = Argon.DefaultValueHandling.Include)
            .IgnoreStackTrace();
    }
}
