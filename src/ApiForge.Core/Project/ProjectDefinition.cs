using ApiForge.Core.Architecture;
using ApiForge.Core.Database;
using ApiForge.Core.Testing;

namespace ApiForge.Core.Project;

public class ProjectDefinition
{
    public required string Name { get; init; }

    public ArchitectureOptions Architecture { get; init; } = new();

    public DatabaseOptions Database { get; init; } = new();

    public TestFramework TestFramework { get; init; } = TestFramework.XUnit;

    public bool UseDocker { get; init; } = true;
}
