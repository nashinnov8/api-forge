using ApiForge.Generator.Abstractions;

namespace ApiForge.Generator.Generation;

public sealed class GenerationPipeline
{
    private readonly IFileSystem _fileSystem;
    private readonly ITemplateRenderer _renderer;

    public GenerationPipeline(IFileSystem fileSystem, ITemplateRenderer renderer)
    {
        _fileSystem = fileSystem;
        _renderer = renderer;
    }

    public IReadOnlyList<string> Run(ProjectContext context)
    {
        var generatedFiles = new List<string>();

        foreach (var sourceFile in _fileSystem.EnumerateFiles(context.TemplatePath))
        {
            var relativePath = Path.GetRelativePath(context.TemplatePath, sourceFile);
            var renderedRelativePath = _renderer.RenderPath(relativePath, context.Tokens);
            var destinationPath = Path.Combine(context.OutputPath, renderedRelativePath);

            var rawContent = _fileSystem.ReadAllText(sourceFile);
            var renderedContent = _renderer.RenderContent(rawContent, context.Tokens);

            _fileSystem.WriteAllText(destinationPath, renderedContent);
            generatedFiles.Add(destinationPath);
        }

        return generatedFiles;
    }
}
