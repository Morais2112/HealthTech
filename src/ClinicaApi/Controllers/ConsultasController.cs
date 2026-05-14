using ClinicaApi.DTOs;
using ClinicaApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaApi.Controllers;

[ApiController]
[Route("consultas")]
[Produces("application/json")]
public class ConsultasController : ControllerBase
{
    private readonly IConsultaService _service;

    public ConsultasController(IConsultaService service)
    {
        _service = service;
    }

    /// <summary>Lista todas as consultas, ordenadas pela data mais recente.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ConsultaResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ConsultaResponseDto>>> Listar()
    {
        var consultas = await _service.ListarAsync();
        return Ok(consultas);
    }

    /// <summary>Busca uma consulta pelo Id.</summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ConsultaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ConsultaResponseDto>> ObterPorId(string id)
    {
        var consulta = await _service.ObterPorIdAsync(id);
        if (consulta is null)
        {
            return NotFound(new { mensagem = $"Consulta com Id '{id}' nao encontrada." });
        }
        return Ok(consulta);
    }

    /// <summary>Agenda uma nova consulta entre um paciente e um medico.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ConsultaResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ConsultaResponseDto>> Criar([FromBody] ConsultaCreateDto dto)
    {
        try
        {
            var criada = await _service.CriarAsync(dto);
            return CreatedAtAction(nameof(ObterPorId), new { id = criada.Id }, criada);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>Atualiza data, status ou observacoes de uma consulta.</summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(string id, [FromBody] ConsultaUpdateDto dto)
    {
        try
        {
            var atualizada = await _service.AtualizarAsync(id, dto);
            if (!atualizada)
            {
                return NotFound(new { mensagem = $"Consulta com Id '{id}' nao encontrada." });
            }
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>Remove uma consulta do sistema.</summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(string id)
    {
        var removida = await _service.RemoverAsync(id);
        if (!removida)
        {
            return NotFound(new { mensagem = $"Consulta com Id '{id}' nao encontrada." });
        }
        return NoContent();
    }
}
