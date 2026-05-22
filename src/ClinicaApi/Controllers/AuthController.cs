using ClinicaApi.DTOs;
using ClinicaApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaApi.Controllers;

[ApiController]
[Route("auth")]
[AllowAnonymous]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    /// <summary>Registra um novo usuario com perfil padrao "Usuario".</summary>
    [HttpPost("registrar")]
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
}
