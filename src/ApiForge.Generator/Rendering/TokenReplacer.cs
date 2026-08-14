namespace ApiForge.Generator.Rendering;

public static class TokenReplacer
{
    public static string Replace(string input, IReadOnlyDictionary<string, string> tokens)
    {
        var result = input;

        foreach (var (key, value) in tokens)
        {
            result = result.Replace($"{{{{{key}}}}}", value);
        }

        return result;
    }
}
