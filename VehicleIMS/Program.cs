using VehicleIMS.Infrastructure;
using VehicleIMS.Infrastructure.Data;
using VehicleIMS.Infrastructure.Middleware;
using System.Text.Json.Serialization;
using VehicleIMS.Infrastructure.Repositories;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// Register infrastructure (DbContext + Identity + all repositories and services)
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandler>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowFrontend");
app.UseAuthentication(); // Add this! Important for Identity
app.UseAuthorization();
app.MapControllers();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<AppDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
    await DBSeeder.SeedAsync(services);
}

app.Run();