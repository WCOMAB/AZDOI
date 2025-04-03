﻿using AZDOI.Services;
using Microsoft.Extensions.Logging.Testing;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace AZDOI.Tests.Unit.Commands;
public class InventoryAllCommandTests
{
    [Theory]

    [InlineData(false, "inventory", "all", "test-org", "/output", "--pat=test-pat")]
    [InlineData(true, "inventory", "all", "test-org", "/output", "--pat=test-pat")]
    [InlineData(true, "inventory", "all", "test-entraid-org", "/output", "--entra-id-auth")]
    public async Task RunAsync(bool outputPathExists, params string[] args)
    {
        // Given
        var (commandApp, testConsole, fakeLog, fakeFileSystem, fakeEnvironment, stopwatchProvider) = ServiceProviderFixture
                                           .GetRequiredService<ICommandApp, TestConsole, FakeLogger<InventoryAllCommand>, FakeFileSystem, FakeEnvironment, StopwatchProvider>(
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
            )
            .DontIgnoreEmptyCollections()
            .AddExtraSettings(setting => setting.DefaultValueHandling = Argon.DefaultValueHandling.Include)
            .IgnoreStackTrace();
    }
}
