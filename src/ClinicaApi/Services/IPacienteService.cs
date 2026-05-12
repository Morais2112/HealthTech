using ClinicaApi.DTOs;

namespace ClinicaApi.Services;

public interface IPacienteService
{
    Task<List<PacienteResponseDto>> ListarAsync();
    Task<PacienteResponseDto?> ObterPorIdAsync(string id);
    Task<PacienteResponseDto> CriarAsync(PacienteCreateDto dto);
    Task<bool> AtualizarAsync(string id, PacienteUpdateDto dto);
    Task<bool> RemoverAsync(string id);
}
