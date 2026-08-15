namespace ApiForge.Generator.Templates;

public sealed class TemplateManifest
{
    public required string Name { get; init; }

    public required string Version { get; init; }

    public required string Type { get; init; }

    public string Architecture { get; init; } = "vertical-slice";

    public IReadOnlyList<string> Features { get; init; } = [];
}
