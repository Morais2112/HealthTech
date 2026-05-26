using ClinicaApi.DTOs;

namespace ClinicaApi.Services;

public interface IUsuarioService
{
    Task<IEnumerable<UsuarioResponseDto>> ListarAsync();
    Task<bool> PromoverAsync(string id);
    Task<bool> RebaixarAsync(string id);
}
