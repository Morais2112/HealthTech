using ClinicaApi.Data;
using ClinicaApi.Models;
using MongoDB.Driver;

namespace ClinicaApi.Repositories;

public class MedicoRepository : IMedicoRepository
{
    private const string CollectionName = "medicos";
    private readonly IMongoCollection<Medico> _collection;

    public MedicoRepository(MongoDbContext context)
    {
        _collection = context.GetCollection<Medico>(CollectionName);
    }

    public async Task<List<Medico>> ListarAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task<Medico?> ObterPorIdAsync(string id)
    {
        return await _collection.Find(m => m.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Medico?> ObterPorCrmAsync(string crm)
    {
        var crmNormalizado = crm.Trim().ToUpperInvariant();
        return await _collection.Find(m => m.Crm == crmNormalizado).FirstOrDefaultAsync();
    }

    public async Task CriarAsync(Medico medico)
    {
        medico.Crm = medico.Crm.Trim().ToUpperInvariant();
        await _collection.InsertOneAsync(medico);
    }

    public async Task<bool> AtualizarAsync(string id, Medico medico)
    {
        var update = Builders<Medico>.Update
            .Set(m => m.Nome, medico.Nome)
            .Set(m => m.Especialidade, medico.Especialidade)
            .Set(m => m.Telefone, medico.Telefone)
            .Set(m => m.Email, medico.Email);

        var result = await _collection.UpdateOneAsync(m => m.Id == id, update);
        return result.MatchedCount > 0;
    }

    public async Task<bool> RemoverAsync(string id)
    {
        var result = await _collection.DeleteOneAsync(m => m.Id == id);
        return result.DeletedCount > 0;
    }
}
