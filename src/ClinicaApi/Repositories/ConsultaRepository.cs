using ClinicaApi.Data;
using ClinicaApi.Models;
using MongoDB.Driver;

namespace ClinicaApi.Repositories;

public class ConsultaRepository : IConsultaRepository
{
    private const string CollectionName = "consultas";
    private readonly IMongoCollection<Consulta> _collection;

    public ConsultaRepository(MongoDbContext context)
    {
        _collection = context.GetCollection<Consulta>(CollectionName);
    }

    public async Task<List<Consulta>> ListarAsync()
    {
        return await _collection.Find(_ => true)
            .SortByDescending(c => c.DataHora)
            .ToListAsync();
    }

    public async Task<Consulta?> ObterPorIdAsync(string id)
    {
        return await _collection.Find(c => c.Id == id).FirstOrDefaultAsync();
    }

    public async Task<bool> ExisteConflitoAsync(string medicoId, DateTime dataHora, string? ignorarId = null)
    {
        var filtroBase = Builders<Consulta>.Filter.Where(c =>
            c.MedicoId == medicoId &&
            c.DataHora == dataHora &&
            c.Status != StatusConsulta.Cancelada);

        if (!string.IsNullOrEmpty(ignorarId))
        {
            filtroBase &= Builders<Consulta>.Filter.Where(c => c.Id != ignorarId);
        }

        return await _collection.Find(filtroBase).AnyAsync();
    }

    public async Task CriarAsync(Consulta consulta)
    {
        await _collection.InsertOneAsync(consulta);
    }

    public async Task<bool> AtualizarAsync(string id, Consulta consulta)
    {
        var update = Builders<Consulta>.Update
            .Set(c => c.DataHora, consulta.DataHora)
            .Set(c => c.Status, consulta.Status)
            .Set(c => c.Observacoes, consulta.Observacoes);

        var result = await _collection.UpdateOneAsync(c => c.Id == id, update);
        return result.MatchedCount > 0;
    }

    public async Task<bool> RemoverAsync(string id)
    {
        var result = await _collection.DeleteOneAsync(c => c.Id == id);
        return result.DeletedCount > 0;
    }
}
