using LinqToDB;
using LinqToDB.AspNet;
using LinqToDB.AspNet.Logging;
using LinqToDB.Async;
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
// builder.Services.AddScoped<CategoryService>();

var app = builder.Build();

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