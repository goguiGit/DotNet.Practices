namespace DotNet.Practices.DynamicMapping.Resolution;

// A malformed model answer is an expected failure of the model, not of the caller; the raw text travels with it for diagnosis.
public sealed class ModelResponseException(string message, string rawResponse, Exception? innerException = null)
    : Exception(message, innerException)
{
    public string RawResponse { get; } = rawResponse;
}
