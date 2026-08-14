using System.Text.Json.Serialization;
using ApiForge.Core.Architecture;
using ApiForge.Core.Database;
using ApiForge.Core.Testing;

namespace ApiForge.Core.Project;

public sealed class ProjectConfigFile
{
    [JsonPropertyName("projectName")]
    public required string ProjectName { get; init; }

    [JsonPropertyName("architecture")]
    public ArchitectureConfig Architecture { get; init; } = new();

    [JsonPropertyName("database")]
    public DatabaseConfig Database { get; init; } = new();

    [JsonPropertyName("testing")]
    public TestingConfig Testing { get; init; } = new();

    [JsonPropertyName("useDocker")]
    public bool UseDocker { get; init; } = true;

    /// <summary>
    /// Converts JSON Config DTO to domain model (ProjectDefinition).
    /// </summary>
    public ProjectDefinition ToDefinition()
    {
        return new ProjectDefinition
        {
            Name = ProjectName,
            Architecture = new ArchitectureOptions
            {
                Style = Enum.TryParse<ArchitectureStyle>(Architecture.Style, ignoreCase: true, out var style)
                    ? style
                    : ArchitectureStyle.VerticalSlice
            },
            Database = new DatabaseOptions
            {
                Provider = Enum.TryParse<DatabaseProvider>(Database.Provider, ignoreCase: true, out var provider)
                    ? provider
                    : DatabaseProvider.PostgreSQL
            },
            TestFramework = Enum.TryParse<TestFramework>(Testing.Framework, ignoreCase: true, out var testFramework)
                ? testFramework
                : TestFramework.XUnit,
            UseDocker = UseDocker
        };
    }

    /// <summary>
    /// Creates a Config DTO from domain model (ProjectDefinition).
    /// </summary>
    public static ProjectConfigFile FromDefinition(ProjectDefinition definition)
    {
        return new ProjectConfigFile
        {
            ProjectName = definition.Name,
            Architecture = new ArchitectureConfig { Style = definition.Architecture.Style.ToString() },
            Database = new DatabaseConfig { Provider = definition.Database.Provider.ToString() },
            Testing = new TestingConfig { Framework = definition.TestFramework.ToString() },
            UseDocker = definition.UseDocker
        };
    }
}

public sealed class ArchitectureConfig
{
    [JsonPropertyName("style")]
    public string Style { get; init; } = nameof(ArchitectureStyle.VerticalSlice);
}

public sealed class DatabaseConfig
{
    [JsonPropertyName("provider")]
    public string Provider { get; init; } = nameof(DatabaseProvider.PostgreSQL);
}

public sealed class TestingConfig
{
    [JsonPropertyName("framework")]
    public string Framework { get; init; } = nameof(TestFramework.XUnit);
}