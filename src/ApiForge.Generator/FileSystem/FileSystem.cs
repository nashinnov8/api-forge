using ApiForge.Generator.Abstractions;

namespace ApiForge.Generator.FileSystem;

public sealed class FileSystem : IFileSystem
{
    public IEnumerable<string> EnumerateFiles(string rootPath) =>
        Directory.EnumerateFiles(rootPath, "*", SearchOption.AllDirectories)
            // bỏ qua manifest và _fragments/, chúng được xử lý riêng
            .Where(f => Path.GetFileName(f) != "template.json")
            .Where(f => !f.Contains($"{Path.DirectorySeparatorChar}_fragments{Path.DirectorySeparatorChar}"));

    public string ReadAllText(string filePath) => File.ReadAllText(filePath);

    public void WriteAllText(string filePath, string content)
    {
        EnsureDirectory(Path.GetDirectoryName(filePath)!);
        File.WriteAllText(filePath, content);
    }

    public void EnsureDirectory(string path) => Directory.CreateDirectory(path);
}
