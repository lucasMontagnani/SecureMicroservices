using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Values;

var builder = WebApplication.CreateBuilder(args);

string authenticationProviderKey = "IdentityApiKey";

builder.Services.AddAuthentication()
 .AddJwtBearer(authenticationProviderKey, x =>
 {
     x.Authority = "https://localhost:5005"; // IDENTITY SERVER URL
                                             //x.RequireHttpsMetadata = false;
     x.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateAudience = false
     };
 });

// Ocelot Basic setup
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddOcelot(); // single ocelot.json file in read-only mode
builder.Services
    .AddOcelot(builder.Configuration);


var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

await app.UseOcelot();

app.Run();
