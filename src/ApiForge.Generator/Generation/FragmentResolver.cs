using ApiForge.Core.Database;
using ApiForge.Core.Project;
using ApiForge.Generator.Abstractions;

namespace ApiForge.Generator.Generation;

public sealed record ActiveFragment(string FilePath, string RelativePath);

public sealed class FragmentResolver
{
    private readonly string _templatePath;
    private readonly ProjectDefinition _definition;

    public FragmentResolver(string templatePath, ProjectDefinition definition)
    {
        _templatePath = templatePath;
        _definition = definition;
    }

    public IEnumerable<ActiveFragment> GetActiveFragments()
    {
        var fragmentsRoot = Path.Combine(_templatePath, "_fragments");
        if (!Directory.Exists(fragmentsRoot))
        {
            yield break;
        }

        foreach (var fragmentDir in Directory.EnumerateDirectories(fragmentsRoot))
        {
            if (!IsFragmentActive(fragmentDir))
            {
                continue;
            }

            foreach (var file in Directory.EnumerateFiles(fragmentDir, "*", SearchOption.AllDirectories))
            {
                var relativePath = Path.GetRelativePath(fragmentDir, file);
                yield return new ActiveFragment(file, relativePath);
            }
        }
    }

    private bool IsFragmentActive(string fragmentDir)
    {
        var name = Path.GetFileName(fragmentDir).ToLowerInvariant();

        return name switch
        {
            "ddd" => _definition.Architecture.UseDdd,
            "cqrs" => _definition.Architecture.UseCqrs,
            "domain-events" => _definition.Architecture.UseDomainEvents,
            "postgres" => _definition.Database.Provider == DatabaseProvider.PostgreSQL,
            "docker" => _definition.UseDocker,
            _ => false
        };
    }
}
