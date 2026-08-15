using ApiForge.Core.Architecture;
using ApiForge.Core.Project;
using ApiForge.Generator.Generation;
using ApiForge.Generator.Rendering;
using ApiForge.Generator.Templates;

namespace ApiForge.Generator.Tests;

public sealed class ProjectGeneratorTests : IDisposable
{
    private readonly string _outputRoot;
    private readonly string _templatesRoot;

    public ProjectGeneratorTests()
    {
        _outputRoot = Path.Combine(Path.GetTempPath(), "apiforge-tests", Guid.NewGuid().ToString("N"));
        _templatesRoot = Path.Combine(AppContext.BaseDirectory, "../../../../../templates");
        Directory.CreateDirectory(_outputRoot);
    }

    public void Dispose()
    {
        if (Directory.Exists(_outputRoot))
        {
            Directory.Delete(_outputRoot, recursive: true);
        }
    }

    private ProjectGenerator CreateGenerator()
    {
        var fileSystem = new ApiForge.Generator.FileSystem.FileSystem();
        var renderer = new TemplateRenderer();
        var pipeline = new GenerationPipeline(fileSystem, renderer);
        var resolver = new TemplateResolver(_templatesRoot);
        return new ProjectGenerator(resolver, pipeline, fileSystem);
    }

    private static ProjectDefinition CreateDefinition() => new() { Name = "OrderService" };

    private static ProjectDefinition CreateDefinitionWithDdd() => new()
    {
        Name = "OrderService",
        Architecture = new ArchitectureOptions { UseDdd = true }
    };

    private static ProjectDefinition CreateDefinitionWithStyle(ArchitectureStyle style) => new()
    {
        Name = "OrderService",
        Architecture = new ArchitectureOptions { Style = style }
    };

    private static ProjectDefinition CreateDefinitionWithoutDocker() => new()
    {
        Name = "OrderService",
        UseDocker = false
    };

    private string OutputProjectPath => Path.Combine(_outputRoot, "OrderService");

    [Fact]
    public void Generate_Creates_Expected_ProjectFiles()
    {
        var generator = CreateGenerator();

        var result = generator.Generate(CreateDefinition(), _outputRoot);

        Assert.True(result.Success, result.ErrorMessage);
        Assert.True(File.Exists(Path.Combine(OutputProjectPath, "src", "OrderService.Api", "OrderService.Api.csproj")));
    }

    [Fact]
    public void Generate_Replaces_Token_In_FileName()
    {
        var generator = CreateGenerator();

        var result = generator.Generate(CreateDefinition(), _outputRoot);

        Assert.True(result.Success, result.ErrorMessage);
        Assert.True(Directory.Exists(Path.Combine(OutputProjectPath, "src", "OrderService.Domain")));
    }

    [Fact]
    public void Generate_Replaces_Token_In_FileContent()
    {
        var generator = CreateGenerator();

        var result = generator.Generate(CreateDefinition(), _outputRoot);

        Assert.True(result.Success, result.ErrorMessage);

        var programPath = Path.Combine(OutputProjectPath, "src", "OrderService.Api", "Program.cs");
        var content = File.ReadAllText(programPath);

        Assert.Contains("OrderService", content);
        Assert.DoesNotContain("{{ProjectName}}", content);
    }

    [Fact]
    public void Generate_Skips_TemplateManifest()
    {
        var generator = CreateGenerator();

        var result = generator.Generate(CreateDefinition(), _outputRoot);

        Assert.True(result.Success, result.ErrorMessage);
        Assert.False(File.Exists(Path.Combine(OutputProjectPath, "template.json")));
    }

    [Fact]
    public void Generate_Returns_All_GeneratedFiles_In_Result()
    {
        var generator = CreateGenerator();

        var result = generator.Generate(CreateDefinition(), _outputRoot);

        Assert.True(result.Success, result.ErrorMessage);

        var templatePath = Path.Combine(_templatesRoot, "api", "vertical-slice");
        var expectedCount = Directory.EnumerateFiles(templatePath, "*", SearchOption.AllDirectories)
            .Count(f => Path.GetFileName(f) != "template.json" && !f.Contains($"{Path.DirectorySeparatorChar}_fragments{Path.DirectorySeparatorChar}"))
            + 1 // .apiforge/project.json
            + Directory.EnumerateFiles(Path.Combine(templatePath, "_fragments", "docker"), "*", SearchOption.AllDirectories).Count()
            + Directory.EnumerateFiles(Path.Combine(templatePath, "_fragments", "postgres"), "*", SearchOption.AllDirectories).Count();

        Assert.Equal(expectedCount, result.GeneratedFiles.Count);
    }

    [Fact]
    public void Generate_VerticalSlice_WithDdd_IncludesEntity()
    {
        var generator = CreateGenerator();

        var result = generator.Generate(CreateDefinitionWithDdd(), _outputRoot);

        Assert.True(result.Success, result.ErrorMessage);
        Assert.True(File.Exists(Path.Combine(OutputProjectPath, "src", "OrderService.Domain", "Common", "Entity.cs")));
    }

    [Fact]
    public void Generate_VerticalSlice_WithoutDdd_ExcludesEntity()
    {
        var generator = CreateGenerator();

        var result = generator.Generate(CreateDefinition(), _outputRoot);

        Assert.True(result.Success, result.ErrorMessage);
        Assert.False(File.Exists(Path.Combine(OutputProjectPath, "src", "OrderService.Domain", "Common", "Entity.cs")));
    }

    [Fact]
    public void Generate_CleanArchitecture_ResolvesTemplate()
    {
        var generator = CreateGenerator();

        var result = generator.Generate(CreateDefinitionWithStyle(ArchitectureStyle.CleanArchitecture), _outputRoot);

        Assert.True(result.Success, result.ErrorMessage);
        Assert.True(File.Exists(Path.Combine(OutputProjectPath, "src", "OrderService.Api", "OrderService.Api.csproj")));
    }

    [Fact]
    public void Generate_ModularMonolith_ResolvesTemplate()
    {
        var generator = CreateGenerator();

        var result = generator.Generate(CreateDefinitionWithStyle(ArchitectureStyle.ModularMonolith), _outputRoot);

        Assert.True(result.Success, result.ErrorMessage);
        Assert.True(File.Exists(Path.Combine(OutputProjectPath, "src", "OrderService.Api", "OrderService.Api.csproj")));
        Assert.True(Directory.Exists(Path.Combine(OutputProjectPath, "src", "Modules", "Orders")));
    }

    [Fact]
    public void Generate_WithoutDocker_ExcludesDockerFiles()
    {
        var generator = CreateGenerator();

        var result = generator.Generate(CreateDefinitionWithoutDocker(), _outputRoot);

        Assert.True(result.Success, result.ErrorMessage);
        Assert.False(File.Exists(Path.Combine(OutputProjectPath, "Dockerfile")));
        Assert.False(File.Exists(Path.Combine(OutputProjectPath, "docker-compose.yml")));
    }
}
