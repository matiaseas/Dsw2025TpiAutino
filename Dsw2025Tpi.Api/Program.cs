using Dsw2025Tpi.Application.Services;
using Dsw2025Tpi.Data;
using Dsw2025Tpi.Data.Repositories;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<Dsw2025TpiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories and services
builder.Services.AddScoped<IRepository, EfRepository>();
builder.Services.AddTransient<ProductsManagementService>();
builder.Services.AddTransient<OrdersManagementService>();

// Add controllers
builder.Services.AddControllers();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
        policy.WithOrigins("https://tu-frontend.com")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// Configure Authentication (JWT)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Add global exception handler
builder.Services.AddProblemDetails();
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("Global", policy =>
    {
        policy.Window = TimeSpan.FromSeconds(30);
        policy.PermitLimit = 100;
    });
    options.RejectionStatusCode = 429; // Too Many Requests
});

var app = builder.Build();

// Seed customers from embedded JSON
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<Dsw2025TpiContext>();
    if (!context.Customers.Any())
    {
        var json = File.ReadAllText("Data/Seed/customers.json");
        var customers = System.Text.Json.JsonSerializer.Deserialize<List<Customer>>(json);
        if (customers != null)
        {
            context.Customers.AddRange(customers);
            context.SaveChanges();
        }
    }
}

// Middleware pipeline
app.UseCors("DefaultCorsPolicy");
app.UseRateLimiter();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Global exception handler
app.UseExceptionHandler("/error");
app.MapControllers();

// Minimal endpoint for error handling
app.MapGet("/error", () => Results.Problem("An unexpected error occurred."));

app.Run();