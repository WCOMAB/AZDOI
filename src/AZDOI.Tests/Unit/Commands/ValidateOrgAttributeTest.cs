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
        var (commandApp, testConsole, fakeFileSystem) = ServiceProviderFixture
                                           .GetRequiredService<ICommandApp, TestConsole, FakeFileSystem>(
                                               services => services.AuthorizedClient()
                                                                   .EntraIdAuthorizedClient()
                                           );

        // When
        fakeFileSystem.CreateDirectory("/output");
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
