using Aspire.Hosting.ApplicationModel;

namespace Aspire.Dex.Hosting;

public class DexConnectorAnnotation(string type, Func<CancellationToken, Task<object>> configurationProvider, string? id = null, string? name = null) : IResourceAnnotation
{
    public string Type { get; } = type;
    public Func<CancellationToken, Task<object>> ConfigurationProvider { get; } = configurationProvider;
    public string? Id { get; } = id;
    public string? Name { get; } = name;
}