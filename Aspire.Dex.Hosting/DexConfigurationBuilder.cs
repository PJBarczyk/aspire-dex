using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Aspire.Dex.Hosting;

internal class DexConfigurationBuilder
{
    private readonly ISerializer serializer = new SerializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();

    private readonly DexConfiguration configuration = new();

    public DexConfigurationBuilder WithIssuer(string issuer)
    {
        configuration.Issuer = issuer;
        return this;
    }

    public DexConfigurationBuilder WithListener(string host, int port)
    {
        configuration.Web.Http = $"{host}:{port}";
        return this;
    }

    public DexConfigurationBuilder WithConnector(string type, string id, string name, object config)
    {
        configuration.Connectors.Add(new()
        {
            Type = type,
            Id = id,
            Name = name,
            Config = config
        });
        return this;
    }

    public DexConfigurationBuilder WithStaticClient(string clientId, string clientSecret, IEnumerable<string> redirectUris)
    {
        configuration.StaticClients.Add(new()
        {
            Id = clientId,
            Name = clientId,
            Secret = clientSecret,
            RedirectUris = [.. redirectUris]
        });

        return this;
    }

    public DexConfigurationBuilder WithStaticPassword(string username, string email, string passwordHash)
    {
        configuration.StaticPasswords.Add(new()
        {
            UserId = Guid.NewGuid().ToString("N"),
            Username = username,
            Email = email,
            Hash = passwordHash
        });
        return this;
    }

    public string Build() => serializer.Serialize(configuration);

    public override string ToString() => Build();
}
