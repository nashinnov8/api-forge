namespace ApiForge.Generator.Templates;

public sealed class TemplateManifest
{
    public string Name { get; init; } = string.Empty;

    public string Version { get; init; } = string.Empty;

    public string Type { get; init; } = string.Empty;

    public string Architecture { get; init; } = "vertical-slice";

    public IReadOnlyList<string> Features { get; init; } = [];
}
