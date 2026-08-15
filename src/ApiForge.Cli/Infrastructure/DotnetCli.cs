using System.Diagnostics;

namespace ApiForge.Cli.Infrastructure;

public static class DotnetCli
{
    public static (bool Success, string Output, string Error) Run(string args, string workingDirectory)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = args,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        using var process = Process.Start(psi)!;
        process.WaitForExit();

        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();

        return (process.ExitCode == 0, output, error);
    }
}
