using Aspire.Hosting.ApplicationModel;

namespace Aspire.Dex.Hosting;

public sealed class DexResource(string name, ParameterResource? clientId, ParameterResource clientSecret) : ContainerResource(name)
{
    private readonly string name = name;

    public EndpointReference HttpEndpoint => new(this, "http");

    public ReferenceExpression AuthorityExpression => ReferenceExpression.Create($"http://{HttpEndpoint.Property(EndpointProperty.HostAndPort)}/dex");

    public ReferenceExpression ClientIdExpression => clientId is not null
        ? ReferenceExpression.Create($"{clientId}")
        : ReferenceExpression.Create($"{name}-client");

    public ReferenceExpression ClientSecretExpression => ReferenceExpression.Create($"{clientSecret}");
}
