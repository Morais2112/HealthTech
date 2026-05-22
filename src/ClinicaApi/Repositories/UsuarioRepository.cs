using ClinicaApi.Data;
using ClinicaApi.Models;
using MongoDB.Driver;

namespace ClinicaApi.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private const string CollectionName = "usuarios";
    private readonly IMongoCollection<Usuario> _collection;

    public UsuarioRepository(MongoDbContext context)
    {
        _collection = context.GetCollection<Usuario>(CollectionName);
        CriarIndiceEmail();
    }

    public async Task<Usuario?> ObterPorEmailAsync(string email)
    {
        var normalizado = email.Trim().ToLowerInvariant();
        return await _collection.Find(u => u.Email == normalizado).FirstOrDefaultAsync();
    }

    public async Task<Usuario?> ObterPorIdAsync(string id)
    {
        return await _collection.Find(u => u.Id == id).FirstOrDefaultAsync();
    }

    public async Task CriarAsync(Usuario usuario)
    {
        usuario.Email = usuario.Email.Trim().ToLowerInvariant();
        await _collection.InsertOneAsync(usuario);
    }

    private void CriarIndiceEmail()
    {
        var keys = Builders<Usuario>.IndexKeys.Ascending(u => u.Email);
        var options = new CreateIndexOptions { Unique = true };
        _collection.Indexes.CreateOne(new CreateIndexModel<Usuario>(keys, options));
    }
}
