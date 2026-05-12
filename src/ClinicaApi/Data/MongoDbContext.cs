using ClinicaApi.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ClinicaApi.Data;

// essa classe centraliza o acesso ao banco
// ideia: em vez de cada repository abrir uma conexao na mao,
// todo mundo pede o IMongoDatabase pra esse contexto aqui
public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoSettings> options)
    {
        // pega as configs ja tipadas (options pattern)
        var settings = options.Value;

        // a propria lib do mongo ja gerencia pool de conexao internamente,
        // entao da pra criar o client uma vez so (singleton no Program.cs)
        var client = new MongoClient(settings.ConnectionString);
        _database = client.GetDatabase(settings.DatabaseName);
    }

    // metodo generico pra pegar qualquer collection,
    // assim os repositories so passam o tipo e o nome
    public IMongoCollection<T> GetCollection<T>(string collectionName)
    {
        return _database.GetCollection<T>(collectionName);
    }
}
