using ClinicaApi.Data;
using ClinicaApi.Models;
using MongoDB.Driver;

namespace ClinicaApi.Repositories;

public class PacienteRepository : IPacienteRepository
{
    private const string CollectionName = "pacientes";
    private readonly IMongoCollection<Paciente> _collection;

    public PacienteRepository(MongoDbContext context)
    {
        _collection = context.GetCollection<Paciente>(CollectionName);
    }

    public async Task<List<Paciente>> ListarAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task<Paciente?> ObterPorIdAsync(string id)
    {
        return await _collection.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Paciente?> ObterPorCpfAsync(string cpf)
    {
        var cpfNormalizado = NormalizarCpf(cpf);
        return await _collection.Find(p => p.Cpf == cpfNormalizado).FirstOrDefaultAsync();
    }

    public async Task CriarAsync(Paciente paciente)
    {
        paciente.Cpf = NormalizarCpf(paciente.Cpf);
        await _collection.InsertOneAsync(paciente);
    }

    public async Task<bool> AtualizarAsync(string id, Paciente paciente)
    {
        var update = Builders<Paciente>.Update
            .Set(p => p.Nome, paciente.Nome)
            .Set(p => p.Telefone, paciente.Telefone)
            .Set(p => p.Email, paciente.Email)
            .Set(p => p.DataNascimento, paciente.DataNascimento);

        var result = await _collection.UpdateOneAsync(p => p.Id == id, update);
        return result.MatchedCount > 0;
    }

    public async Task<bool> RemoverAsync(string id)
    {
        var result = await _collection.DeleteOneAsync(p => p.Id == id);
        return result.DeletedCount > 0;
    }

    private static string NormalizarCpf(string cpf)
    {
        return new string(cpf.Where(char.IsDigit).ToArray());
    }
}
