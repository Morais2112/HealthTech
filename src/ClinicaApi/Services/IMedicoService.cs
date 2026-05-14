using ClinicaApi.DTOs;

namespace ClinicaApi.Services;

public interface IMedicoService
{
    Task<List<MedicoResponseDto>> ListarAsync();
    Task<MedicoResponseDto?> ObterPorIdAsync(string id);
    Task<MedicoResponseDto> CriarAsync(MedicoCreateDto dto);
    Task<bool> AtualizarAsync(string id, MedicoUpdateDto dto);
    Task<bool> RemoverAsync(string id);
}
