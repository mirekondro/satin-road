using System.Text;
using System.Text.Json.Serialization;
using LinqToDB;
using LinqToDB.AspNet;
using LinqToDB.AspNet.Logging;
using LinqToDB.Async;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SatinRoad.Api.Auth;
using SatinRoad.Api.Errors;
using SatinRoad.Core.Auth;
using SatinRoad.Core.Categories;
using SatinRoad.Core.Data;
using SatinRoad.Core.Entities;
using SatinRoad.Core.Listings;
using SatinRoad.Core.Orders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.NumberHandling = JsonNumberHandling.Strict);
builder.Services.ConfigureHttpJsonOptions(o =>
    o.SerializerOptions.NumberHandling = JsonNumberHandling.Strict);
builder.Services.AddOpenApi();

// Database
builder.Services.AddLinqToDBContext<AppDataConnection>((provider, options) =>
    options
        .UsePostgreSQL(builder.Configuration.GetConnectionString("Default")!)
        .UseDefaultLogging(provider));

// Auth (JWT)
var jwt = builder.Configuration.GetSection("Jwt");
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o => o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = jwt["Issuer"],
        ValidAudience = jwt["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)),
    });
builder.Services.AddAuthorization();

// Services
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<IListingRepository, ListingRepository>();
builder.Services.AddScoped<ListingService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddSingleton<IChanceProvider, RandomChanceProvider>();
builder.Services.AddSingleton(new FbiSettings(builder.Configuration.GetValue("Fbi:Chance", 0.01)));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddSingleton<JwtTokenService>();

// Errors: Core exceptions → 400/401/403/404/409
builder.Services.AddExceptionHandler<DomainExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();
app.UseExceptionHandler();

await SeedAdminAsync(app);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(o => o.SwaggerEndpoint("/openapi/v1.json", "Satin Road"));
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Health check – ověří připojení k DB (hodí se později i pro Docker)
app.MapGet("/api/health/db", async (AppDataConnection db) => await db.Categories.CountAsync());

app.Run();

// Vytvoří admina při startu, pokud ještě neexistuje
static async Task SeedAdminAsync(WebApplication app)
{
    var password = app.Configuration["Admin:Password"];
    if (string.IsNullOrEmpty(password)) return;

    using var scope = app.Services.CreateScope();
    var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

    if (await users.GetByUsernameAsync("admin") is null)
        await users.AddAsync(new User { Username = "admin", PasswordHash = hasher.Hash(password), Role = "Admin" });
}
