using ClinicaApi.DTOs;
using ClinicaApi.Models;
using ClinicaApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaApi.Controllers;

[ApiController]
[Route("auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    /// <summary>Registra um novo usuario. O primeiro usuario do sistema vira Admin automaticamente.</summary>
    [HttpPost("registrar")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UsuarioResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UsuarioResponseDto>> Registrar([FromBody] RegistroDto dto)
    {
        try
        {
            var criado = await _service.RegistrarAsync(dto);
            return StatusCode(StatusCodes.Status201Created, criado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>Autentica um usuario e retorna um JWT.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto dto)
    {
        try
        {
            var resultado = await _service.LoginAsync(dto);
            return Ok(resultado);
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(new { mensagem = ex.Message });
        }
    }

    /// <summary>Lista todos os usuarios. Restrito ao perfil Admin.</summary>
    [HttpGet("usuarios")]
    [Authorize(Roles = PerfilUsuario.Admin)]
    [ProducesResponseType(typeof(IEnumerable<UsuarioResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<UsuarioResponseDto>>> ListarUsuarios()
    {
        var usuarios = await _service.ListarUsuariosAsync();
        return Ok(usuarios);
    }

    /// <summary>Promove um usuario ao perfil Admin. Restrito ao perfil Admin.</summary>
    [HttpPost("usuarios/{id}/promover")]
    [Authorize(Roles = PerfilUsuario.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Promover(string id)
    {
        var ok = await _service.PromoverAsync(id);
        if (!ok)
        {
            return NotFound(new { mensagem = $"Usuario com Id '{id}' nao encontrado." });
        }
        return NoContent();
    }

    /// <summary>Rebaixa um usuario para o perfil Usuario. Restrito ao perfil Admin.</summary>
    [HttpPost("usuarios/{id}/rebaixar")]
    [Authorize(Roles = PerfilUsuario.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Rebaixar(string id)
    {
        var ok = await _service.RebaixarAsync(id);
        if (!ok)
        {
            return NotFound(new { mensagem = $"Usuario com Id '{id}' nao encontrado." });
        }
        return NoContent();
    }
}
