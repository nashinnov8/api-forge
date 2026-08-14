using System.Text.Json;
using ApiForge.Core.Generation;
using ApiForge.Core.Project;
using ApiForge.Generator.Abstractions;
using ApiForge.Generator.Templates;

namespace ApiForge.Generator.Generation;

public sealed class ProjectGenerator : IProjectGenerator
{
    private readonly TemplateResolver _resolver;
    private readonly GenerationPipeline _pipeline;
    private readonly IFileSystem _fileSystem;

    public ProjectGenerator(TemplateResolver resolver, GenerationPipeline pipeline, IFileSystem fileSystem)
    {
        _resolver = resolver;
        _pipeline = pipeline;
        _fileSystem = fileSystem;
    }

    public GenerationResult Generate(ProjectDefinition definition, string outputRootPath)
    {
        try
        {
            var templatePath = _resolver.Resolve(definition);
            var outputPath = Path.Combine(outputRootPath, definition.Name);

            var context = new ProjectContext
            {
                Definition = definition,
                TemplatePath = templatePath,
                OutputPath = outputPath
            };

            var files = _pipeline.Run(context);
            
            var configFile = ProjectConfigFile.FromDefinition(definition);
            var configJson = JsonSerializer.Serialize(configFile, new JsonSerializerOptions { WriteIndented = true });
            var configPath = Path.Combine(outputPath, ".apiforge", "project.json");
            _fileSystem.WriteAllText(configPath, configJson);
            var allFiles = files.Append(configPath).ToList();
            
            return GenerationResult.Ok(outputPath, allFiles);
        }
        catch (Exception ex)
        {
            return GenerationResult.Failed(outputRootPath, ex.Message);
        }
    }
}
