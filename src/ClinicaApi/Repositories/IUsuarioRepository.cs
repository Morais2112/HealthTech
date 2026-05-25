using ClinicaApi.Models;

namespace ClinicaApi.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorEmailAsync(string email);
    Task<Usuario?> ObterPorIdAsync(string id);
    Task<IEnumerable<Usuario>> ListarAsync();
    Task<long> ContarAsync();
    Task CriarAsync(Usuario usuario);
    Task<bool> AtualizarPerfilAsync(string id, string perfil);
}
