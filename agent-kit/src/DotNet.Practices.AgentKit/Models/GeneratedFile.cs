namespace DotNet.Practices.AgentKit.Models;

// RelativePath always uses forward slashes; the writer maps it to the platform.
public sealed record GeneratedFile(string RelativePath, string Content);
