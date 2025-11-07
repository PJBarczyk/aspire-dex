using Aspire.Dex.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var dex = builder.AddDex("dex")
    .WithStaticPassword("Dex", "dex@example.com", "password");

var web = builder.AddProject<Projects.Aspire_Dex_Demo_Web>("web")
    .WithReference(dex);

dex.WithRedirectUrl(web.GetEndpoint("https"));
dex.WithRedirectUrl(web.GetEndpoint("http"));

builder.Build().Run();
