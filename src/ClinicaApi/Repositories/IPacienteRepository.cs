using ClinicaApi.Models;

namespace ClinicaApi.Repositories;

public interface IPacienteRepository
{
    Task<List<Paciente>> ListarAsync();
    Task<Paciente?> ObterPorIdAsync(string id);
    Task<Paciente?> ObterPorCpfAsync(string cpf);
    Task CriarAsync(Paciente paciente);
    Task<bool> AtualizarAsync(string id, Paciente paciente);
    Task<bool> RemoverAsync(string id);
}
