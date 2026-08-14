namespace ApiForge.Core.Generation;

public sealed class GenerationResult
{
    public required bool Success { get; init; }

    public required string OutputPath { get; init; }

    public IReadOnlyList<string> GeneratedFiles { get; init; } = [];

    public string? ErrorMessage { get; init; }

    public static GenerationResult Ok(string outputPath, IReadOnlyList<string> files) =>
        new() { Success = true, OutputPath = outputPath, GeneratedFiles = files };

    public static GenerationResult Failed(string outputPath, string error) =>
        new() { Success = false, OutputPath = outputPath, ErrorMessage = error };
}