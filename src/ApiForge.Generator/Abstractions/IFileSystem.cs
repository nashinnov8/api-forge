namespace ApiForge.Generator.Abstractions;

public interface IFileSystem
{
    IEnumerable<string> EnumerateFiles(string rootPath);

    string ReadAllText(string filePath);

    void WriteAllText(string filePath, string content);

    void EnsureDirectory(string path);
}
