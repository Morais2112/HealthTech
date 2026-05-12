using ClinicaApi.DTOs;
using ClinicaApi.Models;
using ClinicaApi.Repositories;

namespace ClinicaApi.Services;

public class PacienteService : IPacienteService
{
    private readonly IPacienteRepository _repository;

    public PacienteService(IPacienteRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<PacienteResponseDto>> ListarAsync()
    {
        var pacientes = await _repository.ListarAsync();
        return pacientes.Select(MapToResponse).ToList();
    }

    public async Task<PacienteResponseDto?> ObterPorIdAsync(string id)
    {
        var paciente = await _repository.ObterPorIdAsync(id);
        return paciente is null ? null : MapToResponse(paciente);
    }

    public async Task<PacienteResponseDto> CriarAsync(PacienteCreateDto dto)
    {
        var existente = await _repository.ObterPorCpfAsync(dto.Cpf);
        if (existente is not null)
        {
            throw new InvalidOperationException("Ja existe um paciente cadastrado com esse CPF.");
        }

        var paciente = new Paciente
        {
            Nome = dto.Nome,
            Cpf = dto.Cpf,
            DataNascimento = dto.DataNascimento,
            Telefone = dto.Telefone,
            Email = dto.Email,
            CriadoEm = DateTime.UtcNow
        };

        await _repository.CriarAsync(paciente);
        return MapToResponse(paciente);
    }

    public async Task<bool> AtualizarAsync(string id, PacienteUpdateDto dto)
    {
        var paciente = new Paciente
        {
            Nome = dto.Nome,
            Telefone = dto.Telefone,
            Email = dto.Email,
            DataNascimento = dto.DataNascimento
        };

        return await _repository.AtualizarAsync(id, paciente);
    }

    public Task<bool> RemoverAsync(string id)
    {
        return _repository.RemoverAsync(id);
    }

    private static PacienteResponseDto MapToResponse(Paciente paciente)
    {
        return new PacienteResponseDto
        {
            Id = paciente.Id ?? string.Empty,
            Nome = paciente.Nome,
            Cpf = paciente.Cpf,
            DataNascimento = paciente.DataNascimento,
            Telefone = paciente.Telefone,
            Email = paciente.Email,
            CriadoEm = paciente.CriadoEm
        };
    }
}
