using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using IdentityServer;
using IdentityServerHost.Quickstart.UI;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// KeyManagement needs a license, so its needed to diable it
builder.Services.AddIdentityServer(options => { options.KeyManagement.Enabled = false; })
                .AddInMemoryClients(IdentityServerConfig.Clients) // Represent applications that can request tokens from IdentityServer
                .AddInMemoryIdentityResources(IdentityServerConfig.IdentityResources) // User informations, like IDs, email, name, claims, etc
                //.AddInMemoryApiResources(IdentityServerConfig.ApiResources)   // Resources in your system that you want to protect
                .AddInMemoryApiScopes(IdentityServerConfig.ApiScopes) // Scoups represent what a client application is allowed to do
                .AddTestUsers(TestUsers.Users) // Users that will try to access the protected resources
                .AddDeveloperSigningCredential(); // Creates temporary keys at compilation time (for dev scenario when don't have license)

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.UseIdentityServer();

app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();
