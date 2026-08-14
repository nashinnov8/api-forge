using ApiForge.Generator.Abstractions;

namespace ApiForge.Generator.Rendering;

public sealed class TemplateRenderer : ITemplateRenderer
{
    public string RenderPath(string path, IReadOnlyDictionary<string, string> tokens) =>
        TokenReplacer.Replace(path, tokens);

    public string RenderContent(string content, IReadOnlyDictionary<string, string> tokens) =>
        TokenReplacer.Replace(content, tokens);
}
