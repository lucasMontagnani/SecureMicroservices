using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Movies.API.Data;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

// Register In-Memory database context
builder.Services.AddDbContext<MoviesAPIContext>(options =>
                    options.UseInMemoryDatabase("MoviesDb"));

// Register Authentication service with Bearer Token
builder.Services.AddAuthentication("Bearer")
                    // Configure support to the Authorization Server
                    .AddJwtBearer("Bearer", options =>  
                    {
                        options.Authority = "https://localhost:5005"; // Address of the IdentityServer (when sending OpenId Calls)
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateAudience = false
                        };
                    });

// Adding a Policy (restriction) specifying which values the specifed claim (first value) are allowed to have
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ClientIdPolicy", policy => policy.RequireClaim("client_id", "movieClient", "movies_mvc_client"));
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();


    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<MoviesAPIContext>();
    // Make sure database is created (if not using migrations)
    context.Database.EnsureCreated();
    // Seed the database
    MoviesContextSeed.SeedAsync(context);
}

app.UseHttpsRedirection();

// Add Authentication Middleware to the pipeline
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
