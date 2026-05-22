using ClinicaApi.DTOs;

namespace ClinicaApi.Services;

public interface IAuthService
{
    Task<UsuarioResponseDto> RegistrarAsync(RegistroDto dto);
    Task<LoginResponseDto> LoginAsync(LoginDto dto);
}
