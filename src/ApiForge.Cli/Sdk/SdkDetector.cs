using ApiForge.Cli.Infrastructure;

namespace ApiForge.Cli.Sdk;

public static class SdkDetector
{
    public static string DetectTargetFramework()
    {
        var currentDir = Directory.GetCurrentDirectory();
        var result = DotnetCli.Run("--list-sdks", currentDir);

        if (result.Success && !string.IsNullOrWhiteSpace(result.Output))
        {
            var version = ParseHighestVersion(result.Output);
            if (version is not null)
            {
                return $"net{version.Major}.{version.Minor}";
            }
        }

        var runtimeResult = DotnetCli.Run("--list-runtimes", currentDir);
        if (runtimeResult.Success && !string.IsNullOrWhiteSpace(runtimeResult.Output))
        {
            var version = ParseHighestAspNetCoreVersion(runtimeResult.Output);
            if (version is not null)
            {
                return $"net{version.Major}.{version.Minor}";
            }
        }

        return "net8.0";
    }

    public static string DetectDotnetVersion()
    {
        var tfm = DetectTargetFramework();
        return tfm.Replace("net", "");
    }

    private static Version? ParseHighestVersion(string output)
    {
        return output
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(line => line.Split(' ')[0])
            .Select(v => Version.TryParse(v, out var version) ? version : null)
            .Where(v => v is not null)
            .OrderByDescending(v => v)
            .FirstOrDefault();
    }

    private static Version? ParseHighestAspNetCoreVersion(string output)
    {
        return output
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(line => line.Contains("ASP.NET Core", StringComparison.OrdinalIgnoreCase))
            .Select(line => line.Split(' ')[1])
            .Select(v => Version.TryParse(v, out var version) ? version : null)
            .Where(v => v is not null)
            .OrderByDescending(v => v)
            .FirstOrDefault();
    }
}
