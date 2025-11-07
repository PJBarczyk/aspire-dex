using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

namespace Aspire.Dex.Hosting;

public static class DexHostingExtensions
{
    internal const int DexPort = 5556;

    public static IResourceBuilder<TDestination> WithReference<TDestination>(this IResourceBuilder<TDestination> builder, IResourceBuilder<DexResource> dex)
        where TDestination : IResourceWithEnvironment
    {
        return builder
            .WithEnvironment("OPENID__AUTHORITY", dex.Resource.AuthorityExpression)
            .WithEnvironment("OPENID__CLIENTID", dex.Resource.ClientIdExpression)
            .WithEnvironment("OPENID__CLIENTSECRET", dex.Resource.ClientSecretExpression);
    }

    public static IResourceBuilder<DexResource> WithStaticPassword(this IResourceBuilder<DexResource> builder, string username, string email, string password)
    {
        return builder.WithAnnotation(new DexStaticPasswordAnnotation(username, email, password));
    }

    public static IResourceBuilder<DexResource> WithRedirectUrl(this IResourceBuilder<DexResource> builder, EndpointReference endpoint, string? path = null)
    {
        return builder.WithAnnotation(new DexRecirectUrlAnnotation(endpoint, path ?? "/signin-oidc"));
    }

    private static async Task<ContainerFile> CreateDexConfigurationFileAsync(DexResource dex, CancellationToken cancellationToken)
    {
        var builder = new DexConfigurationBuilder()
            .WithIssuer($"{dex.HttpEndpoint.Url}/dex")
            .WithListener(host: "0.0.0.0", port: DexPort);

        dex.TryGetAnnotationsOfType<DexRecirectUrlAnnotation>(out var redirectAnnotations);
        var redirectUris = redirectAnnotations?.Select(a => a.RedirectUri.ToString()) ?? [];

        builder.WithStaticClient(
            clientId: (await dex.ClientIdExpression.GetValueAsync(cancellationToken))!,
            clientSecret: (await dex.ClientSecretExpression.GetValueAsync(cancellationToken))!,
            redirectUris: redirectUris);

        if (dex.TryGetAnnotationsOfType<DexConnectorAnnotation>(out var connectorAnnotations))
        {
            foreach (var annotation in connectorAnnotations)
            {
                builder.WithConnector(
                    type: annotation.Type,
                    config: await annotation.ConfigurationProvider.Invoke(cancellationToken),
                    id: annotation.Id ?? Guid.NewGuid().ToString("N"),
                    name: annotation.Name ?? annotation.Type);
            }
        }

        if (dex.TryGetAnnotationsOfType<DexStaticPasswordAnnotation>(out var passwordAnnotations))
        {
            foreach (var annotation in passwordAnnotations)
            {
                builder.WithStaticPassword(
                    username: annotation.Username,
                    email: annotation.Email,
                    passwordHash: BCrypt.Net.BCrypt.HashPassword(annotation.Password));
            }
        }

        return new ContainerFile
        {
            Name = "config.yaml",
            Contents = builder.Build()
        };
    }

    public static IResourceBuilder<DexResource> AddDex(
        this IDistributedApplicationBuilder builder,
        string name,
        IResourceBuilder<ParameterResource>? clientId = null,
        IResourceBuilder<ParameterResource>? clientSecret = null)
    {
        var clientSecretParameter = clientSecret?.Resource ?? ParameterResourceBuilderExtensions.CreateDefaultPasswordParameter(builder, $"{name}-secret", special: false);

        var resource = new DexResource(name, clientId?.Resource, clientSecretParameter);

        return builder.AddResource(resource)
            .WithImage(DexContainerImageTags.Image)
            .WithImageTag(DexContainerImageTags.Tag)
            .WithImageRegistry(DexContainerImageTags.Registry)
            .WithHttpEndpoint(name: "http", targetPort: DexPort)
            .WithUrlForEndpoint("http", endpoint => new ResourceUrlAnnotation()
             {
                 DisplayOrder = 100,
                 DisplayText = "Discovery URL",
                 Url = $"{endpoint.Url}/dex/.well-known/openid-configuration"
            })
            .WithHttpHealthCheck("/dex", 200, "http")
            .WithArgs(["dex", "serve", "/etc/dex/config.yaml"])
            .WithContainerFiles("/etc/dex", async (context, ct) => [await CreateDexConfigurationFileAsync(resource, ct)]);
    }
}