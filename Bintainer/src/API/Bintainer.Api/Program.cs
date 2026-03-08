using Bintainer.Api.Extensions;
using Bintainer.Api.Middleware;
using Bintainer.Common.Application;
using Bintainer.Common.Infrastructure;
using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Modules.ActivityLog.Infrastructure;
using Bintainer.Modules.Catalog.Infrastructure;
using Bintainer.Modules.Inventory.Infrastructure;
using Bintainer.Modules.Reports.Infrastructure;
using Bintainer.Modules.Users.Infrastructure;
using Microsoft.OpenApi.Models;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Configuration.AddModuleConfiguration(["users", "inventory", "catalog", "activity"]);

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
    Bintainer.Modules.Inventory.Application.AssemblyReference.Assembly,
    Bintainer.Modules.Catalog.Application.AssemblyReference.Assembly,
    Bintainer.Modules.ActivityLog.Application.AssemblyReference.Assembly,
    Bintainer.Modules.Reports.Application.AssemblyReference.Assembly
]);

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddInfrastructure(connectionString, InventoryModule.ConfigureConsumers);

builder.Services.AddUsersModule(builder.Configuration);
builder.Services.AddInventoryModule(builder.Configuration);
builder.Services.AddCatalogModule(builder.Configuration);
builder.Services.AddActivityLogModule(builder.Configuration);
builder.Services.AddReportsModule(builder.Configuration);

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ApplyMigrations();
}

app.UseExceptionHandler();

app.UseStaticFiles();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();

app.Run();
