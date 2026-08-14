using ApiForge.Core.Project;

namespace ApiForge.Generator.Generation;

public sealed class ProjectContext
{
    public required ProjectDefinition Definition { get; init; }

    public required string TemplatePath { get; init; }

    public required string OutputPath { get; init; }

    public IReadOnlyDictionary<string, string> Tokens => new Dictionary<string, string>
    {
        ["ProjectName"] = Definition.Name
    };
}
