using System.Text;
using DotNet.Practices.AgentKit.Models;

namespace DotNet.Practices.AgentKit.Output;

public static class KitWriter
{
    private static readonly UTF8Encoding Utf8WithoutBom = new(encoderShouldEmitUTF8Identifier: false);

    // Writes every file and returns the relative paths whose content changed.
    public static IReadOnlyList<string> Write(string repositoryRoot, IReadOnlyList<GeneratedFile> files)
    {
        var changed = new List<string>();
        foreach (var file in files)
        {
            var path = Resolve(repositoryRoot, file.RelativePath);
            if (File.Exists(path) && File.ReadAllText(path, Utf8WithoutBom) == file.Content)
            {
                continue;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, file.Content, Utf8WithoutBom);
            changed.Add(file.RelativePath);
        }

        return changed;
    }

    // Returns the relative paths that are missing or differ on disk, without touching anything.
    public static IReadOnlyList<string> Check(string repositoryRoot, IReadOnlyList<GeneratedFile> files) =>
        files.Where(file =>
            {
                var path = Resolve(repositoryRoot, file.RelativePath);
                return !File.Exists(path) || File.ReadAllText(path, Utf8WithoutBom) != file.Content;
            })
            .Select(file => file.RelativePath)
            .ToList();

    private static string Resolve(string repositoryRoot, string relativePath)
    {
        var root = Path.GetFullPath(repositoryRoot);
        var full = Path.GetFullPath(Path.Combine(root, relativePath.Replace('/', Path.DirectorySeparatorChar)));
        return full.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.Ordinal)
            ? full
            : throw new ArgumentException($"'{relativePath}' escapes the repository root.", nameof(relativePath));
    }
}
