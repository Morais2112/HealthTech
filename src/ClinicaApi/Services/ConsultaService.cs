using ClinicaApi.DTOs;
using ClinicaApi.Models;
using ClinicaApi.Repositories;
using MongoDB.Bson;

namespace ClinicaApi.Services;

public class ConsultaService : IConsultaService
{
    private readonly IConsultaRepository _consultaRepository;
    private readonly IPacienteRepository _pacienteRepository;
    private readonly IMedicoRepository _medicoRepository;

    public ConsultaService(
        IConsultaRepository consultaRepository,
        IPacienteRepository pacienteRepository,
        IMedicoRepository medicoRepository)
    {
        _consultaRepository = consultaRepository;
        _pacienteRepository = pacienteRepository;
        _medicoRepository = medicoRepository;
    }

    public async Task<List<ConsultaResponseDto>> ListarAsync()
    {
        var consultas = await _consultaRepository.ListarAsync();
        var resultado = new List<ConsultaResponseDto>(consultas.Count);

        foreach (var consulta in consultas)
        {
            resultado.Add(await MontarResponseAsync(consulta));
        }

        return resultado;
    }

    public async Task<ConsultaResponseDto?> ObterPorIdAsync(string id)
    {
        var consulta = await _consultaRepository.ObterPorIdAsync(id);
        return consulta is null ? null : await MontarResponseAsync(consulta);
    }

    public async Task<ConsultaResponseDto> CriarAsync(ConsultaCreateDto dto)
    {
        ValidarObjectId(dto.PacienteId, nameof(dto.PacienteId));
        ValidarObjectId(dto.MedicoId, nameof(dto.MedicoId));

        if (dto.DataHora < DateTime.UtcNow)
        {
            throw new InvalidOperationException("A data da consulta deve ser futura.");
        }

        var paciente = await _pacienteRepository.ObterPorIdAsync(dto.PacienteId)
            ?? throw new InvalidOperationException("Paciente nao encontrado.");

        var medico = await _medicoRepository.ObterPorIdAsync(dto.MedicoId)
            ?? throw new InvalidOperationException("Medico nao encontrado.");

        if (await _consultaRepository.ExisteConflitoAsync(dto.MedicoId, dto.DataHora))
        {
            throw new InvalidOperationException("Ja existe uma consulta agendada para esse medico nesse horario.");
        }

        var consulta = new Consulta
        {
            PacienteId = paciente.Id!,
            MedicoId = medico.Id!,
            DataHora = dto.DataHora,
            Observacoes = dto.Observacoes,
            Status = StatusConsulta.Agendada,
            CriadoEm = DateTime.UtcNow
        };

        await _consultaRepository.CriarAsync(consulta);
        return await MontarResponseAsync(consulta);
    }

    public async Task<bool> AtualizarAsync(string id, ConsultaUpdateDto dto)
    {
        var consulta = await _consultaRepository.ObterPorIdAsync(id);
        if (consulta is null)
        {
            return false;
        }

        if (dto.Status == StatusConsulta.Agendada && dto.DataHora < DateTime.UtcNow)
        {
            throw new InvalidOperationException("Nao e possivel reagendar para uma data no passado.");
        }

        if (dto.Status != StatusConsulta.Cancelada &&
            await _consultaRepository.ExisteConflitoAsync(consulta.MedicoId, dto.DataHora, id))
        {
            throw new InvalidOperationException("Ja existe outra consulta agendada para esse medico nesse horario.");
        }

        consulta.DataHora = dto.DataHora;
        consulta.Status = dto.Status;
        consulta.Observacoes = dto.Observacoes;

        return await _consultaRepository.AtualizarAsync(id, consulta);
    }

    public Task<bool> RemoverAsync(string id)
    {
        return _consultaRepository.RemoverAsync(id);
    }

    private async Task<ConsultaResponseDto> MontarResponseAsync(Consulta consulta)
    {
        var paciente = await _pacienteRepository.ObterPorIdAsync(consulta.PacienteId);
        var medico = await _medicoRepository.ObterPorIdAsync(consulta.MedicoId);

        return new ConsultaResponseDto
        {
            Id = consulta.Id ?? string.Empty,
            PacienteId = consulta.PacienteId,
            PacienteNome = paciente?.Nome ?? "(paciente removido)",
            MedicoId = consulta.MedicoId,
            MedicoNome = medico?.Nome ?? "(medico removido)",
            MedicoEspecialidade = medico?.Especialidade ?? string.Empty,
            DataHora = consulta.DataHora,
            Status = consulta.Status,
            Observacoes = consulta.Observacoes,
            CriadoEm = consulta.CriadoEm
        };
    }

    private static void ValidarObjectId(string id, string campo)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            throw new InvalidOperationException($"O campo {campo} nao e um identificador valido.");
        }
    }
}
