using DotNet.Practices.AgentKit.Generators;
using DotNet.Practices.AgentKit.Generators.Interfaces;
using DotNet.Practices.AgentKit.Output;
using DotNet.Practices.AgentKit.Source;

var check = args.Contains("--check");
var repositoryRoot = Path.GetFullPath(args.FirstOrDefault(argument => !argument.StartsWith("--", StringComparison.Ordinal)) ?? Directory.GetCurrentDirectory());
var sourceDirectory = Path.Combine(repositoryRoot, "agent-kit", "source");

if (!Directory.Exists(sourceDirectory))
{
    Console.Error.WriteLine($"No agent-kit source at {sourceDirectory}. Usage: dotnet run -- <repositoryRoot> [--check]");
    return 2;
}

IGenerator[] generators = [new ClaudeCodeGenerator(), new AgentsMdGenerator(), new CursorGenerator(), new CopilotGenerator()];
var source = KitSourceReader.Read(sourceDirectory);
var files = generators.SelectMany(generator => generator.Generate(source)).ToList();

if (check)
{
    var drift = KitWriter.Check(repositoryRoot, files);
    foreach (var path in drift)
    {
        Console.WriteLine($"out of date: {path}");
    }

    Console.WriteLine(drift.Count == 0 ? $"{files.Count} generated files are up to date." : $"{drift.Count} of {files.Count} generated files are out of date. Run without --check to regenerate.");
    return drift.Count == 0 ? 0 : 1;
}

var changed = KitWriter.Write(repositoryRoot, files);
foreach (var path in changed)
{
    Console.WriteLine($"written: {path}");
}

Console.WriteLine($"{changed.Count} of {files.Count} generated files changed ({string.Join(", ", generators.Select(generator => generator.Tool))}).");
return 0;
