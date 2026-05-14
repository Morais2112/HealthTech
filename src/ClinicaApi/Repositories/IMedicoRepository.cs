using ClinicaApi.Models;

namespace ClinicaApi.Repositories;

public interface IMedicoRepository
{
    Task<List<Medico>> ListarAsync();
    Task<Medico?> ObterPorIdAsync(string id);
    Task<Medico?> ObterPorCrmAsync(string crm);
    Task CriarAsync(Medico medico);
    Task<bool> AtualizarAsync(string id, Medico medico);
    Task<bool> RemoverAsync(string id);
}
