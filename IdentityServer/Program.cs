using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using IdentityServer;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// KeyManagement needs a license, so its needed to diable it
builder.Services.AddIdentityServer(options => { options.KeyManagement.Enabled = false; })
                .AddInMemoryClients(IdentityServerConfig.Clients) // Represent applications that can request tokens from IdentityServer
                //.AddInMemoryIdentityResources(IdentityServerConfig.IdentityResources) // User informations, like IDs, email, name, claims, etc
                //.AddInMemoryApiResources(IdentityServerConfig.ApiResources)   // Resources in your system that you want to protect
                .AddInMemoryApiScopes(IdentityServerConfig.ApiScopes) // Scoups represent what a client application is allowed to do
                //.AddTestUsers(IdentityServerConfig.TestUsers) // Users that will try to access the protected resources
                .AddDeveloperSigningCredential(); // Creates temporary keys at compilation time (for dev scenario when don't have license)

var app = builder.Build();

app.UseIdentityServer();

app.Run();
