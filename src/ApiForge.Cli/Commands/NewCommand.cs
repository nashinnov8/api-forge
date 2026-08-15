using System.CommandLine;
using ApiForge.Cli.Infrastructure;
using ApiForge.Cli.Sdk;
using ApiForge.Cli.Wizard;
using ApiForge.Generator.Abstractions;

namespace ApiForge.Cli.Commands;

public static class NewCommand
{
    public static Command Create(IProjectGenerator generator, string outputRootPath)
    {
        var nameArgument = new Argument<string?>("name", "Tên project cần generate")
        {
            Arity = ArgumentArity.ZeroOrOne
        };

        var command = new Command("new", "Generate một .NET API project mới")
        {
            nameArgument
        };

        command.SetHandler((string? name) =>
        {
            var definition = InteractiveWizard.Run(name);
            if (definition is null)
            {
                return;
            }

            var targetFramework = SdkDetector.DetectTargetFramework();
            var dotnetVersion = SdkDetector.DetectDotnetVersion();

            definition = definition with
            {
                TargetFramework = targetFramework,
                DotnetVersion = dotnetVersion
            };

            Console.WriteLine($"Generating {definition.Name}...");

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

            RunDotnetCommand($"new sln -n {definition.Name}", result.OutputPath);
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
            Console.WriteLine($"  dotnet run --project src/{definition.Name}.Api");

        }, nameArgument);

        return command;
    }

    private static void RunDotnetCommand(string dotnetArgs, string workingDirectory)
    {
        Console.WriteLine($"Running dotnet {dotnetArgs}...");

        var (success, output, error) = DotnetCli.Run(dotnetArgs, workingDirectory);

        if (success)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✓ dotnet {dotnetArgs} succeeded");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"✗ dotnet {dotnetArgs} failed");
            if (!string.IsNullOrWhiteSpace(output))
            {
                Console.WriteLine(output);
            }
            if (!string.IsNullOrWhiteSpace(error))
            {
                Console.WriteLine(error);
            }
        }
        Console.ResetColor();
    }
}
