using ApiForge.Core.Architecture;
using ApiForge.Core.Database;
using ApiForge.Core.Project;
using ApiForge.Core.Testing;

namespace ApiForge.Core.Tests;

public class ProjectDefinitionTests
{
    [Fact]
    public void Default_Architecture_Is_VerticalSlice()
    {
        var definition = new ProjectDefinition { Name = "OrderService" };

        Assert.Equal(ArchitectureStyle.VerticalSlice, definition.Architecture.Style);
    }

    [Fact]
    public void Default_Database_Is_PostgreSql()
    {
        var definition = new ProjectDefinition { Name = "OrderService" };

        Assert.Equal(DatabaseProvider.PostgreSQL, definition.Database.Provider);
    }

    [Fact]
    public void Default_TestFramework_Is_XUnit()
    {
        var definition = new ProjectDefinition { Name = "OrderService" };

        Assert.Equal(TestFramework.XUnit, definition.TestFramework);
    }

    [Fact]
    public void Default_UseDocker_Is_True()
    {
        var definition = new ProjectDefinition { Name = "OrderService" };

        Assert.True(definition.UseDocker);
    }

    [Fact]
    public void Name_Is_Required()
    {
        // required member -> lỗi compile nếu thiếu Name khi khởi tạo,
        // test này chỉ xác nhận giá trị được gán đúng khi truyền vào
        var definition = new ProjectDefinition { Name = "OrderService" };

        Assert.Equal("OrderService", definition.Name);
    }

    [Fact]
    public void Default_TargetFramework_Is_Net80()
    {
        var definition = new ProjectDefinition { Name = "OrderService" };

        Assert.Equal("net8.0", definition.TargetFramework);
    }

    [Fact]
    public void Default_DotnetVersion_Is_80()
    {
        var definition = new ProjectDefinition { Name = "OrderService" };

        Assert.Equal("8.0", definition.DotnetVersion);
    }
}