using YamlDotNet.Serialization;

namespace Aspire.Dex.Hosting;

internal class DexConfiguration
{
    public string? Issuer { get; set; }
    public Web Web { get; set; } = new();
    public Storage Storage { get; set; } = new();
    public List<Connector> Connectors { get; set; } = [];
    public List<StaticClient> StaticClients { get; set; } = [];
    [YamlMember(Alias = "enablePasswordDB")]
    public bool EnablePasswordDb { get; set; } = true;
    public List<StaticPassword> StaticPasswords { get; set; } = [];
}

internal class Web
{
    public string? Http { get; set; }
}

internal class Storage
{
    public string Type { get; set; } = "memory";
}

internal class Connector
{
    public required string Type { get; set; }
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required object Config { get; set; }
}

internal class StaticClient
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Secret { get; set; }
    [YamlMember(Alias = "redirectURIs")]
    public required List<string> RedirectUris { get; set; }
}

internal class StaticPassword
{
    [YamlMember(Alias = "userID")]
    public required string UserId { get; set; }
    public required string Email { get; set; }
    public required string Username { get; set; }
    public required string Hash { get; set; }
}
