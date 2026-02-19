using System.Text.Json;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Identity.Web;
using Serilog;
using VueWebEnterpriseSQL.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// SERILOG — Replace the default .NET logger
// ==========================================
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// ==========================================
// 1. REGISTER SERVICES
// ==========================================

// Add all layers (Application, Infrastructure) + API services (CORS, Exception Handler)
builder.Services.AddWebServices(builder.Configuration);

// Add Authentication (Entra ID)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization();
builder.Services.AddControllers();

// Swagger with API Versioning support (dropdown per version)
builder.Services.AddSwaggerWithVersioning();

var app = builder.Build();

// ==========================================
// 2. CONFIGURE PIPELINE
// ==========================================

// Global Exception Handler — must be early in the pipeline
app.UseExceptionHandler();

// Serilog HTTP request logging (replaces default Microsoft request logs)
app.UseSerilogRequestLogging();

// Enable Swagger ONLY in Development (with version dropdown)
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerWithVersioning();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

// --- HANGFIRE DASHBOARD (Start) ---
// Only enable if Hangfire was registered (requires SQL Server to be running).
// Hangfire is skipped when SQL Server is unreachable — see Infrastructure/DependencyInjection.cs
if (app.Services.GetService<IBackgroundJobClient>() is not null)
{
    app.UseHangfireDashboard();
}

app.MapControllers();

// Health Check endpoint — returns JSON { status: "Healthy" }
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";

        var result = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                duration = e.Value.Duration.TotalMilliseconds + "ms"
            })
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(result, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        }));
    }
});

app.Run();