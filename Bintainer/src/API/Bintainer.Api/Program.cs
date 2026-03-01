using Bintainer.Api.Extensions;
using Bintainer.Api.Middleware;
using Bintainer.Common.Application;
using Bintainer.Common.Infrastructure;
using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Modules.Inventory.Infrastructure;
using Bintainer.Modules.Users.Infrastructure;
using Microsoft.OpenApi.Models;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Configuration.AddModuleConfiguration(["users", "inventory"]);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Bintainer API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter 'Bearer {token}'",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddApplication([
    Bintainer.Modules.Users.Application.AssemblyReference.Assembly,
    Bintainer.Modules.Inventory.Application.AssemblyReference.Assembly
]);

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddInfrastructure(connectionString);

builder.Services.AddUsersModule(builder.Configuration);
builder.Services.AddInventoryModule(builder.Configuration);

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ApplyMigrations();
}

app.UseExceptionHandler();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();

app.Run();
