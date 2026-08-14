using ApiForge.Core.Generation;
using ApiForge.Core.Project;

namespace ApiForge.Generator.Abstractions;

public interface IProjectGenerator
{
    GenerationResult Generate(ProjectDefinition definition, string outputRootPath);
}
