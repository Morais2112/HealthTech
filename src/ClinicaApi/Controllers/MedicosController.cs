using ClinicaApi.DTOs;
using ClinicaApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaApi.Controllers;

[ApiController]
[Authorize]
[Route("medicos")]
[Produces("application/json")]
public class MedicosController : ControllerBase
{
    private readonly IMedicoService _service;

    public MedicosController(IMedicoService service)
    {
        _service = service;
    }

    /// <summary>Lista todos os medicos cadastrados.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MedicoResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MedicoResponseDto>>> Listar()
    {
        var medicos = await _service.ListarAsync();
        return Ok(medicos);
    }

    /// <summary>Busca um medico pelo seu Id.</summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MedicoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MedicoResponseDto>> ObterPorId(string id)
    {
        var medico = await _service.ObterPorIdAsync(id);
        if (medico is null)
        {
            return NotFound(new { mensagem = $"Medico com Id '{id}' nao encontrado." });
        }
        return Ok(medico);
    }

    /// <summary>Cadastra um novo medico.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(MedicoResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MedicoResponseDto>> Criar([FromBody] MedicoCreateDto dto)
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

    /// <summary>Atualiza os dados de um medico existente.</summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(string id, [FromBody] MedicoUpdateDto dto)
    {
        var atualizado = await _service.AtualizarAsync(id, dto);
        if (!atualizado)
        {
            return NotFound(new { mensagem = $"Medico com Id '{id}' nao encontrado." });
        }
        return NoContent();
    }

    /// <summary>Remove um medico do sistema.</summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(string id)
    {
        var removido = await _service.RemoverAsync(id);
        if (!removido)
        {
            return NotFound(new { mensagem = $"Medico com Id '{id}' nao encontrado." });
        }
        return NoContent();
    }
}
