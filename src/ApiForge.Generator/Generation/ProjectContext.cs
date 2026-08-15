using ApiForge.Core.Project;

namespace ApiForge.Generator.Generation;

public sealed class ProjectContext
{
    public ProjectDefinition Definition { get; init; } = null!;

    public string TemplatePath { get; init; } = string.Empty;

    public string OutputPath { get; init; } = string.Empty;

    public IReadOnlyDictionary<string, string> Tokens => new Dictionary<string, string>
    {
        ["ProjectName"] = Definition.Name,
        ["TargetFramework"] = Definition.TargetFramework,
        ["DotnetVersion"] = Definition.DotnetVersion
    };
}
