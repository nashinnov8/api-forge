using ApiForge.Core.Architecture;
using ApiForge.Core.Project;

namespace ApiForge.Generator.Templates;

public sealed class TemplateResolver
{
    private readonly string _templatesRoot;

    public TemplateResolver(string templatesRoot)
    {
        _templatesRoot = templatesRoot;
    }

    public string Resolve(ProjectDefinition definition)
    {
        var templateDir = definition.Architecture.Style switch
        {
            ArchitectureStyle.VerticalSlice => "vertical-slice",
            ArchitectureStyle.CleanArchitecture => "clean-architecture",
            ArchitectureStyle.ModularMonolith => "modular-monolith",
            _ => "vertical-slice"
        };

        var path = Path.Combine(_templatesRoot, "api", templateDir);

        if (!Directory.Exists(path))
        {
            if (Directory.Exists(_templatesRoot) &&
                File.Exists(Path.Combine(_templatesRoot, "template.json")))
            {
                path = _templatesRoot;
            }
            else
            {
                throw new DirectoryNotFoundException($"Template not found at: {path}");
            }
        }

        return path;
    }

    public TemplateManifest ReadManifest(string templatePath)
    {
        var manifestPath = Path.Combine(templatePath, "template.json");
        var json = File.ReadAllText(manifestPath);

        return System.Text.Json.JsonSerializer.Deserialize<TemplateManifest>(json,
            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? throw new InvalidOperationException("Invalid template manifest.");
    }
}
