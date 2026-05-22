using ClinicaApi.Models;

namespace ClinicaApi.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorEmailAsync(string email);
    Task<Usuario?> ObterPorIdAsync(string id);
    Task CriarAsync(Usuario usuario);
}
