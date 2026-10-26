using System.Net;

namespace DotNet.Practices.Abstractions.RequestPipeline;

public class RequestContext(string path, string? user)
{
    public string Path { get; } = path;

    public string? User { get; } = user;

    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.NotFound;

    public List<string> Trace { get; } = [];
}
