using ClinicaApi.DTOs;

namespace ClinicaApi.Services;

public interface IAuthService
{
    Task<UsuarioResponseDto> RegistrarAsync(RegistroDto dto);
    Task<LoginResponseDto> LoginAsync(LoginDto dto);
    Task<IEnumerable<UsuarioResponseDto>> ListarUsuariosAsync();
    Task<bool> PromoverAsync(string id);
    Task<bool> RebaixarAsync(string id);
}
