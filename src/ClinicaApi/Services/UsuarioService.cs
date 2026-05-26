using ClinicaApi.DTOs;
using ClinicaApi.Models;
using ClinicaApi.Repositories;

namespace ClinicaApi.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repository;

    public UsuarioService(IUsuarioRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<UsuarioResponseDto>> ListarAsync()
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
}
