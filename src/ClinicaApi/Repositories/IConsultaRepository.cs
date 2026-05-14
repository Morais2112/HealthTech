using ClinicaApi.Models;

namespace ClinicaApi.Repositories;

public interface IConsultaRepository
{
    Task<List<Consulta>> ListarAsync();
    Task<Consulta?> ObterPorIdAsync(string id);
    Task<bool> ExisteConflitoAsync(string medicoId, DateTime dataHora, string? ignorarId = null);
    Task CriarAsync(Consulta consulta);
    Task<bool> AtualizarAsync(string id, Consulta consulta);
    Task<bool> RemoverAsync(string id);
}
