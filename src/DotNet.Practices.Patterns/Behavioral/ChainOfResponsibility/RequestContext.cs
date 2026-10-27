using System.Net;

namespace DotNet.Practices.Patterns.Behavioral.ChainOfResponsibility;

public class RequestContext(string path, string? user)
{
    public string Path { get; } = path;

    public string? User { get; } = user;

    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.NotFound;

    public List<string> Trace { get; } = [];
}
