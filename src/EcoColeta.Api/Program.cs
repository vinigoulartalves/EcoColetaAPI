using System.Text;
using EcoColeta.Api.Configurations;
using EcoColeta.Api.Data;
using EcoColeta.Api.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEcoColetaServices(builder.Configuration);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<IConfiguration>((options, configuration) =>
    {
        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
            ?? throw new InvalidOperationException("Configuração JWT não encontrada.");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Emissor,
            ValidAudience = jwtSettings.Audiencia,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.ChaveSecreta)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Valida a configuração final, incluindo overrides da factory de testes.
if (Encoding.UTF8.GetByteCount(app.Configuration["Jwt:ChaveSecreta"] ?? "") < 32)
    throw new InvalidOperationException("Defina Jwt__ChaveSecreta com pelo menos 32 bytes.");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbInitializer.SeedAsync(context, app.Configuration);
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseDefaultFiles();
app.UseStaticFiles();

if (app.Configuration.GetValue<bool>("Swagger:Enabled"))
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "EcoColeta API v1");
        options.RoutePrefix = "swagger";
    });
}

if (app.Configuration.GetValue<bool>("Https:UseRedirection"))
    app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health/live", () => Results.Ok(new { status = "Healthy" }));
app.MapGet("/health/ready", async (AppDbContext db, IHostEnvironment env, IConfiguration config) =>
{
    try
    {
        if (!await db.Database.CanConnectAsync())
            return Results.Json(new { status = "Unhealthy" }, statusCode: 503);
        return Results.Ok(new
        {
            status = "Healthy", database = "Connected", environment = env.EnvironmentName,
            version = config["APP_VERSION"] ?? "local"
        });
    }
    catch
    {
        return Results.Json(new { status = "Unhealthy" }, statusCode: 503);
    }
});

app.Run();

public partial class Program { }
