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
        _templatesRoot = Path.Combine(AppContext.BaseDirectory, "../../../../../templates/api/default");
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

        var expectedCount = Directory.EnumerateFiles(_templatesRoot, "*", SearchOption.AllDirectories)
            .Count(f => Path.GetFileName(f) != "template.json") + 1; // +1 for .apiforge/project.json

        Assert.Equal(expectedCount, result.GeneratedFiles.Count);
    }
}
