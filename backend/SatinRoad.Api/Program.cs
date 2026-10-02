using LinqToDB;
using LinqToDB.AspNet;
using LinqToDB.AspNet.Logging;
using LinqToDB.Async;
using SatinRoad.Api.Errors;
using SatinRoad.Core.Categories;
using SatinRoad.Core.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Database
builder.Services.AddLinqToDBContext<AppDataConnection>((provider, options) =>
    options
        .UsePostgreSQL(builder.Configuration.GetConnectionString("Default")!)
        .UseDefaultLogging(provider));

// Services
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<CategoryService>();

// Errors: Core exceptions → 400/404/409
builder.Services.AddExceptionHandler<DomainExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(o => o.SwaggerEndpoint("/openapi/v1.json", "Satin Road"));
}

app.UseAuthorization();
app.MapControllers();

// Health check – ověří připojení k DB (hodí se později i pro Docker)
app.MapGet("/api/health/db", async (AppDataConnection db) => await db.Categories.CountAsync());

app.Run();