using ApiForge.Core.Generation;
using ApiForge.Core.Project;
using ApiForge.Generator.Abstractions;
using ApiForge.Generator.Templates;

namespace ApiForge.Generator.Generation;

public sealed class ProjectGenerator : IProjectGenerator
{
    private readonly TemplateResolver _resolver;
    private readonly GenerationPipeline _pipeline;

    public ProjectGenerator(TemplateResolver resolver, GenerationPipeline pipeline)
    {
        _resolver = resolver;
        _pipeline = pipeline;
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

            return GenerationResult.Ok(outputPath, files);
        }
        catch (Exception ex)
        {
            return GenerationResult.Failed(outputRootPath, ex.Message);
        }
    }
}
