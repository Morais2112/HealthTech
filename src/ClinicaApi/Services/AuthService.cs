using ClinicaApi.DTOs;
using ClinicaApi.Models;
using ClinicaApi.Repositories;

namespace ClinicaApi.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _repository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(
        IUsuarioRepository repository,
        ITokenService tokenService,
        IPasswordHasher passwordHasher)
    {
        _repository = repository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
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
            SenhaHash = _passwordHasher.Hash(dto.Senha),
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

    public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
    {
        var usuario = await _repository.ObterPorEmailAsync(dto.Email);
        if (usuario is null || !_passwordHasher.Verificar(dto.Senha, usuario.SenhaHash))
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
