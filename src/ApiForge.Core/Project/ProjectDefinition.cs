using ApiForge.Core.Architecture;
using ApiForge.Core.Database;
using ApiForge.Core.Testing;

namespace ApiForge.Core.Project;

public record ProjectDefinition
{
    public string Name { get; init; } = string.Empty;

    public ArchitectureOptions Architecture { get; init; } = new();

    public DatabaseOptions Database { get; init; } = new();

    public TestFramework TestFramework { get; init; } = TestFramework.XUnit;

    public bool UseDocker { get; init; } = true;

    public string TargetFramework { get; init; } = "net8.0";

    public string DotnetVersion { get; init; } = "8.0";
}
