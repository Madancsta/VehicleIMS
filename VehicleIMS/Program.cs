using VehicleIMS.Infrastructure;
using VehicleIMS.Infrastructure.Data;
using VehicleIMS.Infrastructure.Middleware;
using VehicleIMS.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register infrastructure (DbContext + Identity)
builder.Services.AddInfrastructure(builder.Configuration);
//builder.Services.AddScoped<IPartRepository, PartRepository>();
//builder.Services.AddScoped<IPartService, PartService>();


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

app.UseCors("Frontend");
app.UseAuthentication(); // Add this! Important for Identity
app.UseAuthorization();
app.MapControllers();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // Optional: Apply migrations or ensure database is created
    var dbContext = services.GetRequiredService<AppDbContext>();
    await dbContext.Database.EnsureCreatedAsync(); // or .MigrateAsync()

    // Seed admin user
    await DBSeeder.SeedAsync(services);
}

app.Run();