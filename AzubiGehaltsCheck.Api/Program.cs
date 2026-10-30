using AzubiGehaltsCheck.Infrastructure;
using AzubiGehaltsCheck.Infrastructure.Data;
using AzubiGehaltsCheck.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.AddServiceDefaults();

builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Infrastructure (PostgreSQL + Identity)
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// this makes sure the db migrates automatically so you dont have to do it manual every time you update
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    var db = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();

    await db.Database.MigrateAsync();
}

// Aspire health endpoints
app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapGroup("/auth")
    .MapIdentityApi<ApplicationUser>();

app.MapControllers();

app.Run();