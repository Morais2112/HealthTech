using System.Text;
using ClinicaApi.Configuration;
using ClinicaApi.Data;
using ClinicaApi.Repositories;
using ClinicaApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var mongoSettings = new MongoSettings
{
    ConnectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING")
        ?? builder.Configuration["MongoSettings:ConnectionString"]
        ?? string.Empty,
    DatabaseName = Environment.GetEnvironmentVariable("MONGO_DATABASE_NAME")
        ?? builder.Configuration["MongoSettings:DatabaseName"]
        ?? string.Empty
};

if (string.IsNullOrWhiteSpace(mongoSettings.ConnectionString) ||
    string.IsNullOrWhiteSpace(mongoSettings.DatabaseName))
{
    throw new InvalidOperationException(
        "MongoSettings nao configurado. Defina as variaveis MONGO_CONNECTION_STRING e MONGO_DATABASE_NAME ou preencha o appsettings.");
}

var jwtSettings = new JwtSettings
{
    Secret = Environment.GetEnvironmentVariable("JWT_SECRET")
        ?? builder.Configuration["JwtSettings:Secret"]
        ?? string.Empty,
    Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
        ?? builder.Configuration["JwtSettings:Issuer"]
        ?? "ClinicaApi",
    Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
        ?? builder.Configuration["JwtSettings:Audience"]
        ?? "ClinicaClient",
    ExpiresMinutes = int.TryParse(Environment.GetEnvironmentVariable("JWT_EXPIRES_MINUTES"), out var min)
        ? min
        : int.TryParse(builder.Configuration["JwtSettings:ExpiresMinutes"], out var minCfg) ? minCfg : 120
};

if (string.IsNullOrWhiteSpace(jwtSettings.Secret) || jwtSettings.Secret.Length < 32)
{
    throw new InvalidOperationException(
        "JWT_SECRET nao configurado ou muito curto. Use uma string com pelo menos 32 caracteres.");
}

builder.Services.Configure<MongoSettings>(opts =>
{
    opts.ConnectionString = mongoSettings.ConnectionString;
    opts.DatabaseName = mongoSettings.DatabaseName;
});

builder.Services.Configure<JwtSettings>(opts =>
{
    opts.Secret = jwtSettings.Secret;
    opts.Issuer = jwtSettings.Issuer;
    opts.Audience = jwtSettings.Audience;
    opts.ExpiresMinutes = jwtSettings.ExpiresMinutes;
});

builder.Services.AddSingleton<MongoDbContext>();

builder.Services.AddScoped<IPacienteRepository, PacienteRepository>();
builder.Services.AddScoped<IPacienteService, PacienteService>();

builder.Services.AddScoped<IMedicoRepository, MedicoRepository>();
builder.Services.AddScoped<IMedicoService, MedicoService>();

builder.Services.AddScoped<IConsultaRepository, ConsultaRepository>();
builder.Services.AddScoped<IConsultaService, ConsultaService>();

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<ITokenService, TokenService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization();

const string CorsPolicyName = "AllowFrontend";
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Health Tech API",
        Version = "v1",
        Description = "API REST da clinica medica Health Tech (Pacientes, Medicos e Consultas).",
        Contact = new OpenApiContact
        {
            Name = "Mateus Morais Lopes",
            Email = "mateusmoraislopes4@gmail.com"
        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Cole apenas o token JWT (sem o prefixo Bearer)."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Health Tech API v1");
    c.RoutePrefix = "swagger";
});

app.UseCors(CorsPolicyName);

var frontendPath = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "..", "frontend"));
if (Directory.Exists(frontendPath))
{
    var fileProvider = new PhysicalFileProvider(frontendPath);
    app.UseDefaultFiles(new DefaultFilesOptions { FileProvider = fileProvider });
    app.UseStaticFiles(new StaticFileOptions { FileProvider = fileProvider });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/health", () => Results.Ok(new { status = "ok", api = "Health Tech API", versao = "v1" }));

app.MapControllers();

app.Run();
