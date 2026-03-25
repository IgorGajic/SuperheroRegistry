using Microsoft.EntityFrameworkCore;
using SuperheroRegistry.Application.Interfaces;
using SuperheroRegistry.Application.Services;
using SuperheroRegistry.Infrastructure.Persistence;
using SuperheroRegistry.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add controllers to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection for repositories and services
builder.Services.AddScoped<IHeroRepository, HeroRepository>();
builder.Services.AddScoped<IHeroService, HeroService>();

// Build
var app = builder.Build();

// Swagger middleware for API documentation in development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();