using ClinicaApi.DTOs;

namespace ClinicaApi.Services;

public interface IConsultaService
{
    Task<List<ConsultaResponseDto>> ListarAsync();
    Task<ConsultaResponseDto?> ObterPorIdAsync(string id);
    Task<ConsultaResponseDto> CriarAsync(ConsultaCreateDto dto);
    Task<bool> AtualizarAsync(string id, ConsultaUpdateDto dto);
    Task<bool> RemoverAsync(string id);
}
