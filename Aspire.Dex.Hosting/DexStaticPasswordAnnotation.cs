using Aspire.Hosting.ApplicationModel;

namespace Aspire.Dex.Hosting;

public class DexStaticPasswordAnnotation(string username, string email, string password) : IResourceAnnotation
{
    public string Username { get; } = username;
    public string Email { get; } = email;
    public string Password { get; } = password;
}
