using ClinicaApi.Configuration;
using ClinicaApi.Data;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------------------------
// Config do Mongo
// -----------------------------------------------------------------------------
// Le da secao "MongoSettings" do appsettings, mas se existir variavel de ambiente
// MONGO_CONNECTION_STRING / MONGO_DATABASE_NAME, ela tem prioridade.
// (o professor pediu pra nao deixar a string hardcoded)
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
    // se nao tem nem env var nem appsettings.Development.json eh melhor falhar logo
    throw new InvalidOperationException(
        "MongoSettings nao configurado. Defina as variaveis MONGO_CONNECTION_STRING e MONGO_DATABASE_NAME ou preencha o appsettings.");
}

// registra o options pattern pra quem precisar injetar IOptions<MongoSettings>
builder.Services.Configure<MongoSettings>(opts =>
{
    opts.ConnectionString = mongoSettings.ConnectionString;
    opts.DatabaseName = mongoSettings.DatabaseName;
});

// MongoDbContext como singleton: a lib do mongo ja eh thread-safe e reaproveita conexao
builder.Services.AddSingleton<MongoDbContext>();

// -----------------------------------------------------------------------------
// CORS - precisa liberar pro frontend html+js conseguir bater na API
// -----------------------------------------------------------------------------
// (em prod a gente restringiria pra dominio especifico, mas em dev libera geral)
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

// -----------------------------------------------------------------------------
// Controllers + Swagger
// -----------------------------------------------------------------------------
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

    // inclui os comentarios XML que o build gera, ai o swagger mostra as descricoes
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// -----------------------------------------------------------------------------
// Pipeline HTTP
// -----------------------------------------------------------------------------
// Swagger so em dev por padrao, mas pro trabalho vou deixar sempre ligado
// (vai facilitar a apresentacao do professor)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Clinica API v1");
    c.RoutePrefix = "swagger";
});

app.UseCors(CorsPolicyName);

// endpoint bobinho de healthcheck so pra confirmar que ta no ar
app.MapGet("/", () => Results.Ok(new { status = "ok", api = "Clinica API", versao = "v1" }));

app.MapControllers();

app.Run();
