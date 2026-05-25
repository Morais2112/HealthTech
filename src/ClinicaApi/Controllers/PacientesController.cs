using ClinicaApi.DTOs;
using ClinicaApi.Models;
using ClinicaApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaApi.Controllers;

[ApiController]
[Authorize]
[Route("pacientes")]
[Produces("application/json")]
public class PacientesController : ControllerBase
{
    private readonly IPacienteService _service;

    public PacientesController(IPacienteService service)
    {
        _service = service;
    }

    /// <summary>Lista todos os pacientes cadastrados.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PacienteResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PacienteResponseDto>>> Listar()
    {
        var pacientes = await _service.ListarAsync();
        return Ok(pacientes);
    }

    /// <summary>Busca um paciente pelo seu Id.</summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PacienteResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PacienteResponseDto>> ObterPorId(string id)
    {
        var paciente = await _service.ObterPorIdAsync(id);
        if (paciente is null)
        {
            return NotFound(new { mensagem = $"Paciente com Id '{id}' nao encontrado." });
        }
        return Ok(paciente);
    }

    /// <summary>Cadastra um novo paciente.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(PacienteResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PacienteResponseDto>> Criar([FromBody] PacienteCreateDto dto)
    {
        try
        {
            var criado = await _service.CriarAsync(dto);
            return CreatedAtAction(nameof(ObterPorId), new { id = criado.Id }, criado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>Atualiza os dados de um paciente existente.</summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(string id, [FromBody] PacienteUpdateDto dto)
    {
        var atualizado = await _service.AtualizarAsync(id, dto);
        if (!atualizado)
        {
            return NotFound(new { mensagem = $"Paciente com Id '{id}' nao encontrado." });
        }
        return NoContent();
    }

    /// <summary>Remove um paciente do sistema. Restrito ao perfil Admin.</summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = PerfilUsuario.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(string id)
    {
        var removido = await _service.RemoverAsync(id);
        if (!removido)
        {
            return NotFound(new { mensagem = $"Paciente com Id '{id}' nao encontrado." });
        }
        return NoContent();
    }
}
