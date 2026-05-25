using ClinicaApi.DTOs;
using ClinicaApi.Models;
using ClinicaApi.Repositories;

namespace ClinicaApi.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _repository;
    private readonly ITokenService _tokenService;

    public AuthService(IUsuarioRepository repository, ITokenService tokenService)
    {
        _repository = repository;
        _tokenService = tokenService;
    }

    public async Task<UsuarioResponseDto> RegistrarAsync(RegistroDto dto)
    {
        var existente = await _repository.ObterPorEmailAsync(dto.Email);
        if (existente is not null)
        {
            throw new InvalidOperationException("Ja existe um usuario com esse email.");
        }

        var total = await _repository.ContarAsync();
        var perfil = total == 0 ? PerfilUsuario.Admin : PerfilUsuario.Usuario;

        var usuario = new Usuario
        {
            Nome = dto.Nome.Trim(),
            Email = dto.Email.Trim().ToLowerInvariant(),
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha),
            Perfil = perfil,
            CriadoEm = DateTime.UtcNow
        };

        await _repository.CriarAsync(usuario);

        return new UsuarioResponseDto
        {
            Id = usuario.Id ?? string.Empty,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Perfil = usuario.Perfil
        };
    }

    public async Task<IEnumerable<UsuarioResponseDto>> ListarUsuariosAsync()
    {
        var usuarios = await _repository.ListarAsync();
        return usuarios.Select(u => new UsuarioResponseDto
        {
            Id = u.Id ?? string.Empty,
            Nome = u.Nome,
            Email = u.Email,
            Perfil = u.Perfil
        });
    }

    public async Task<bool> PromoverAsync(string id)
    {
        var usuario = await _repository.ObterPorIdAsync(id);
        if (usuario is null)
        {
            return false;
        }
        if (usuario.Perfil == PerfilUsuario.Admin)
        {
            return true;
        }
        return await _repository.AtualizarPerfilAsync(id, PerfilUsuario.Admin);
    }

    public async Task<bool> RebaixarAsync(string id)
    {
        var usuario = await _repository.ObterPorIdAsync(id);
        if (usuario is null)
        {
            return false;
        }
        if (usuario.Perfil == PerfilUsuario.Usuario)
        {
            return true;
        }
        return await _repository.AtualizarPerfilAsync(id, PerfilUsuario.Usuario);
    }

    public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
    {
        var usuario = await _repository.ObterPorEmailAsync(dto.Email);
        if (usuario is null || !BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.SenhaHash))
        {
            throw new InvalidOperationException("Email ou senha invalidos.");
        }

        var (token, expiraEm) = _tokenService.Gerar(usuario);

        return new LoginResponseDto
        {
            Token = token,
            ExpiraEm = expiraEm,
            Usuario = new UsuarioResponseDto
            {
                Id = usuario.Id ?? string.Empty,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Perfil = usuario.Perfil
            }
        };
    }
}
