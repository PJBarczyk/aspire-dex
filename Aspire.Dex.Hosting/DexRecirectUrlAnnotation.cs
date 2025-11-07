using Aspire.Hosting.ApplicationModel;

namespace Aspire.Dex.Hosting;

public class DexRecirectUrlAnnotation(EndpointReference endpoint, string path) : IResourceAnnotation
{
    public EndpointReference EndpointReference => endpoint;
    public string Path => path;

    public Uri RedirectUri => new UriBuilder(endpoint.Url)
    {
        Path = path
    }.Uri;
}
