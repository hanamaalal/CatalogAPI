using Microsoft.EntityFrameworkCore;
using ProductCatalog.Api.Data;
using ProductCatalog.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuration de la connexion SQL Server
var connectionString =
    builder.Configuration.GetConnectionString("CatalogDbConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Activation de Swagger et de l'explorateur d'API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger pour tester l'API localement
app.UseSwagger();
app.UseSwaggerUI();

// Création de la base de données au démarrage
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Crée le schéma si nécessaire
    db.Database.EnsureCreated();
}

// 1. Endpoint de santé (Healthcheck)
app.MapGet("/health", () =>
    Results.Ok(new
    {
        status = "Healthy",
        timestamp = DateTime.UtcNow
    }))
    .WithName("HealthCheck");

// 2. Liste des produits
app.MapGet("/api/products", async (AppDbContext db) =>
    await db.Products.AsNoTracking().ToListAsync())
    .WithName("GetAllProducts");

// 3. Ajout d'un produit
app.MapPost("/api/products", async (Product product, AppDbContext db) =>
{
    db.Products.Add(product);
    await db.SaveChangesAsync();

    return Results.Created($"/api/products/{product.Id}", product);
})
.WithName("CreateProduct");

app.Run();