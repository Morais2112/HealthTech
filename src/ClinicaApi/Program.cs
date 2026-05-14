using ClinicaApi.Configuration;
using ClinicaApi.Data;
using ClinicaApi.Repositories;
using ClinicaApi.Services;
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

builder.Services.Configure<MongoSettings>(opts =>
{
    opts.ConnectionString = mongoSettings.ConnectionString;
    opts.DatabaseName = mongoSettings.DatabaseName;
});

builder.Services.AddSingleton<MongoDbContext>();

builder.Services.AddScoped<IPacienteRepository, PacienteRepository>();
builder.Services.AddScoped<IPacienteService, PacienteService>();

builder.Services.AddScoped<IMedicoRepository, MedicoRepository>();
builder.Services.AddScoped<IMedicoService, MedicoService>();

builder.Services.AddScoped<IConsultaRepository, ConsultaRepository>();
builder.Services.AddScoped<IConsultaService, ConsultaService>();

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
        Title = "Clinica API",
        Version = "v1",
        Description = "API REST para gerenciamento de uma clinica medica (Pacientes, Medicos e Consultas).",
        Contact = new OpenApiContact
        {
            Name = "Mateus Morais Lopes",
            Email = "mateusmoraislopes4@gmail.com"
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
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Clinica API v1");
    c.RoutePrefix = "swagger";
});

app.UseCors(CorsPolicyName);

app.MapGet("/", () => Results.Ok(new { status = "ok", api = "Clinica API", versao = "v1" }));

app.MapControllers();

app.Run();
