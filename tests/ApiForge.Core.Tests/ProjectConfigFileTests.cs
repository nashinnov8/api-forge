using ApiForge.Core.Architecture;
using ApiForge.Core.Database;
using ApiForge.Core.Project;
using ApiForge.Core.Testing;

namespace ApiForge.Core.Tests;

public class ProjectConfigFileTests
{
    [Fact]
    public void ToDefinition_Converts_Config_To_ProjectDefinition_Correctly()
    {
        var config = new ProjectConfigFile
        {
            ProjectName = "PaymentService",
            Architecture = new ArchitectureConfig { Style = "VerticalSlice" },
            Database = new DatabaseConfig { Provider = "PostgreSQL" },
            Testing = new TestingConfig { Framework = "XUnit" },
            UseDocker = true
        };

        var definition = config.ToDefinition();

        Assert.Equal("PaymentService", definition.Name);
        Assert.Equal(ArchitectureStyle.VerticalSlice, definition.Architecture.Style);
        Assert.Equal(DatabaseProvider.PostgreSQL, definition.Database.Provider);
        Assert.Equal(TestFramework.XUnit, definition.TestFramework);
        Assert.True(definition.UseDocker);
    }

    [Fact]
    public void FromDefinition_Converts_ProjectDefinition_To_Config_Correctly()
    {
        var definition = new ProjectDefinition
        {
            Name = "OrderService"
        };

        var config = ProjectConfigFile.FromDefinition(definition);

        Assert.Equal("OrderService", config.ProjectName);
        Assert.Equal("VerticalSlice", config.Architecture.Style);
        Assert.Equal("PostgreSQL", config.Database.Provider);
        Assert.Equal("XUnit", config.Testing.Framework);
        Assert.True(config.UseDocker);
    }
}
