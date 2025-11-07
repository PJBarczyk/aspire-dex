# Dex Identity Provider for .NET Aspire

A basic implementation of a [Dex](https://dexidp.io/) identity provider as a .NET Aspire container resource, enabling simple OpenID Connect authentication in [run mode](https://learn.microsoft.com/en-us/dotnet/aspire/architecture/overview#run-mode):

```csharp
// Define Dex identity provider with local accounts
var dex = builder.AddDex("dex")
    .WithStaticPassword("Dex", "dex@example.com", "password");

// Reference Dex from a web project, injecting OpenID Connect configuration
var web = builder.AddProject<Projects.Example>("web")
    .WithReference(dex);

// Configure redirect URLs for Dex
dex.WithRedirectUrl(web.GetEndpoint("https"));
dex.WithRedirectUrl(web.GetEndpoint("http"));
```

Referencing the Dex resource injects a `OpenId` section into the web project's configuration, which can be used to configure ASP.NET Core authentication:

```csharp
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        options.Authority = builder.Configuration["OpenId:Authority"];
        options.ClientId = builder.Configuration["OpenId:ClientId"];
        options.ClientSecret = builder.Configuration["OpenId:ClientSecret"];
        ...
    });
```

## Technical Details

Methods `WithStaticPassword` and `WithRedirectUrl` attach attributes which are then used to create the Dex configuration file. This happens after the containers are created and endpoints allocated, but before the containers are started.

## Limitations

As endpoint allocation is only done in run mode, this resource cannot be used in publish mode.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.