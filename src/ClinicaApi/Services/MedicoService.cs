using ClinicaApi.DTOs;
using ClinicaApi.Models;
using ClinicaApi.Repositories;

namespace ClinicaApi.Services;

public class MedicoService : IMedicoService
{
    private readonly IMedicoRepository _repository;

    public MedicoService(IMedicoRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<MedicoResponseDto>> ListarAsync()
    {
        var medicos = await _repository.ListarAsync();
        return medicos.Select(MapToResponse).ToList();
    }

    public async Task<MedicoResponseDto?> ObterPorIdAsync(string id)
    {
        var medico = await _repository.ObterPorIdAsync(id);
        return medico is null ? null : MapToResponse(medico);
    }

    public async Task<MedicoResponseDto> CriarAsync(MedicoCreateDto dto)
    {
        var existente = await _repository.ObterPorCrmAsync(dto.Crm);
        if (existente is not null)
        {
            throw new InvalidOperationException("Ja existe um medico cadastrado com esse CRM.");
        }

        var medico = new Medico
        {
            Nome = dto.Nome,
            Crm = dto.Crm,
            Especialidade = dto.Especialidade,
            Telefone = dto.Telefone,
            Email = dto.Email,
            CriadoEm = DateTime.UtcNow
        };

        await _repository.CriarAsync(medico);
        return MapToResponse(medico);
    }

    public async Task<bool> AtualizarAsync(string id, MedicoUpdateDto dto)
    {
        var medico = new Medico
        {
            Nome = dto.Nome,
            Especialidade = dto.Especialidade,
            Telefone = dto.Telefone,
            Email = dto.Email
        };

        return await _repository.AtualizarAsync(id, medico);
    }

    public Task<bool> RemoverAsync(string id)
    {
        return _repository.RemoverAsync(id);
    }

    private static MedicoResponseDto MapToResponse(Medico medico)
    {
        return new MedicoResponseDto
        {
            Id = medico.Id ?? string.Empty,
            Nome = medico.Nome,
            Crm = medico.Crm,
            Especialidade = medico.Especialidade,
            Telefone = medico.Telefone,
            Email = medico.Email,
            CriadoEm = medico.CriadoEm
        };
    }
}
