using System.Text.Json;
using ApiForge.Core.Project;

namespace ApiForge.Generator.Templates;

public sealed class TemplateResolver
{
    private readonly string _templatesRoot;

    public TemplateResolver(string templatesRoot)
    {
        _templatesRoot = templatesRoot;
    }

    // V1: luôn resolve về templates/api/default, chưa cần logic chọn theo Architecture
    public string Resolve(ProjectDefinition definition)
    {
        var path = Path.Combine(_templatesRoot, "api", "default");

        if (!Directory.Exists(path))
        {
            // Nếu templatesRoot đã trỏ thẳng vào một template (có template.json)
            // thì dùng trực tiếp thay vì nối thêm api/default.
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

        return JsonSerializer.Deserialize<TemplateManifest>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? throw new InvalidOperationException("Invalid template manifest.");
    }
}
