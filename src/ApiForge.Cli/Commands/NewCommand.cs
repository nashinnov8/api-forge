using System.CommandLine;
using ApiForge.Cli.Infrastructure;
using ApiForge.Cli.Sdk;
using ApiForge.Cli.Wizard;
using ApiForge.Core.Architecture;
using ApiForge.Core.Database;
using ApiForge.Core.Project;
using ApiForge.Core.Testing;
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

        var architectureOption = new Option<string?>(
            "--architecture",
            "Architecture style: vertical-slice, clean-architecture, modular-monolith");

        var databaseOption = new Option<string?>(
            "--database",
            "Database provider: postgres, none");

        var dddOption = new Option<bool?>(
            "--ddd",
            "Include DDD building blocks (true/false)");

        var cqrsOption = new Option<bool?>(
            "--cqrs",
            "Include CQRS abstractions (true/false)");

        var domainEventsOption = new Option<bool?>(
            "--domain-events",
            "Include domain events (true/false)");

        var dockerOption = new Option<bool?>(
            "--docker",
            "Include Docker setup (true/false)");

        var testFrameworkOption = new Option<string?>(
            "--test-framework",
            "Test framework: xunit");

        var command = new Command("new", "Generate một .NET API project mới")
        {
            nameArgument,
            architectureOption,
            databaseOption,
            dddOption,
            cqrsOption,
            domainEventsOption,
            dockerOption,
            testFrameworkOption
        };

        command.SetHandler(
            (string? name, string? architecture, string? database, bool? ddd,
             bool? cqrs, bool? domainEvents, bool? docker, string? testFramework) =>
            {
                var definition = BuildDefinition(
                    name, architecture, database, ddd, cqrs, domainEvents, docker, testFramework);

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

                GenerateProject(definition, generator, outputRootPath);
            },
            nameArgument, architectureOption, databaseOption, dddOption,
            cqrsOption, domainEventsOption, dockerOption, testFrameworkOption);

        return command;
    }

    private static ProjectDefinition? BuildDefinition(
        string? name,
        string? architecture,
        string? database,
        bool? ddd,
        bool? cqrs,
        bool? domainEvents,
        bool? docker,
        string? testFramework)
    {
        var hasAnyFlag = architecture is not null
            || database is not null
            || ddd is not null
            || cqrs is not null
            || domainEvents is not null
            || docker is not null
            || testFramework is not null;

        if (hasAnyFlag)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("✗ --name is required in non-interactive mode");
                Console.ResetColor();
                return null;
            }

            return new ProjectDefinition
            {
                Name = name,
                Architecture = new ArchitectureOptions
                {
                    Style = ParseArchitecture(architecture),
                    UseDdd = ddd ?? false,
                    UseCqrs = cqrs ?? false,
                    UseDomainEvents = domainEvents ?? false
                },
                Database = new DatabaseOptions { Provider = ParseDatabase(database) },
                TestFramework = ParseTestFramework(testFramework),
                UseDocker = docker ?? true
            };
        }

        return InteractiveWizard.Run(name);
    }

    private static ArchitectureStyle ParseArchitecture(string? value) =>
        value?.ToLowerInvariant() switch
        {
            "vertical-slice" => ArchitectureStyle.VerticalSlice,
            "clean-architecture" => ArchitectureStyle.CleanArchitecture,
            "modular-monolith" => ArchitectureStyle.ModularMonolith,
            null => ArchitectureStyle.VerticalSlice,
            _ => ArchitectureStyle.VerticalSlice
        };

    private static DatabaseProvider ParseDatabase(string? value) =>
        value?.ToLowerInvariant() switch
        {
            "postgres" => DatabaseProvider.PostgreSQL,
            "none" => DatabaseProvider.None,
            null => DatabaseProvider.PostgreSQL,
            _ => DatabaseProvider.PostgreSQL
        };

    private static TestFramework ParseTestFramework(string? value) =>
        value?.ToLowerInvariant() switch
        {
            "xunit" => TestFramework.XUnit,
            null => TestFramework.XUnit,
            _ => TestFramework.XUnit
        };

    private static void GenerateProject(
        ProjectDefinition definition,
        IProjectGenerator generator,
        string outputRootPath)
    {
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
