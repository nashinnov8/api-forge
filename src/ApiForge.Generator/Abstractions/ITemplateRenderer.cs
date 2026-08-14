namespace ApiForge.Generator.Abstractions;

public interface ITemplateRenderer
{
    string RenderPath(string path, IReadOnlyDictionary<string, string> tokens);

    string RenderContent(string content, IReadOnlyDictionary<string, string> tokens);
}
