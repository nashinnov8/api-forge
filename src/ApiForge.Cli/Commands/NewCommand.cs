using System.CommandLine;
using ApiForge.Core.Project;
using ApiForge.Generator.Abstractions;

namespace ApiForge.Cli.Commands;

public static class NewCommand
{
    public static Command Create(IProjectGenerator generator, string outputRootPath)
    {
        var nameArgument = new Argument<string>("name", "Tên project cần generate");

        var command = new Command("new", "Generate một .NET API project mới")
        {
            nameArgument
        };

        command.SetHandler((string name) =>
        {
            // V1: hardcode ProjectDefinition — chưa có wizard, chỉ để xác nhận pipeline chạy đúng
            var definition = new ProjectDefinition { Name = name };

            Console.WriteLine($"Generating {name}...");

            var result = generator.Generate(definition, outputRootPath);

            if (!result.Success)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ Generation failed: {result.ErrorMessage}");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✓ Generated {result.GeneratedFiles.Count} files at {result.OutputPath}");
            Console.ResetColor();

            RunDotnetCommand($"new sln -n {name}", result.OutputPath);
            var csprojFiles = Directory.EnumerateFiles(result.OutputPath, "*.csproj", SearchOption.AllDirectories);

            foreach (var csprojPath in csprojFiles)
            {
                var relativePath = Path.GetRelativePath(result.OutputPath, csprojPath);
                RunDotnetCommand($"sln add {relativePath}", result.OutputPath);
            }
            RunDotnetCommand("restore", result.OutputPath);
            RunDotnetCommand("build", result.OutputPath);

            Console.WriteLine();
            Console.WriteLine("Next steps:");
            Console.WriteLine($"  cd {result.OutputPath}");
            Console.WriteLine($"  dotnet run --project src/{name}.Api");

        }, nameArgument);

        return command;
    }

    private static void RunDotnetCommand(string dotnetArgs, string workingDirectory)
    {
        Console.WriteLine($"Running dotnet {dotnetArgs}...");

        var psi = new System.Diagnostics.ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = dotnetArgs,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        using var process = System.Diagnostics.Process.Start(psi);
        process!.WaitForExit();

        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();

        if (process.ExitCode == 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✓ dotnet {dotnetArgs} succeeded");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"✗ dotnet {dotnetArgs} failed");
            Console.WriteLine(error);
            Console.ResetColor();
        }
    }
}
