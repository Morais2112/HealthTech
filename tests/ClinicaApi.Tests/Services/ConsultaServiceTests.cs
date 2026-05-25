using ClinicaApi.DTOs;
using ClinicaApi.Models;
using ClinicaApi.Repositories;
using ClinicaApi.Services;
using MongoDB.Bson;
using Moq;
using Xunit;

namespace ClinicaApi.Tests.Services;

public class ConsultaServiceTests
{
    private readonly Mock<IConsultaRepository> _consultaRepo = new();
    private readonly Mock<IPacienteRepository> _pacienteRepo = new();
    private readonly Mock<IMedicoRepository> _medicoRepo = new();
    private readonly ConsultaService _service;

    private static readonly string PacienteIdValido = ObjectId.GenerateNewId().ToString();
    private static readonly string MedicoIdValido = ObjectId.GenerateNewId().ToString();

    public ConsultaServiceTests()
    {
        _service = new ConsultaService(_consultaRepo.Object, _pacienteRepo.Object, _medicoRepo.Object);
    }

    private void ConfigurarPacienteEMedicoExistentes()
    {
        _pacienteRepo.Setup(r => r.ObterPorIdAsync(PacienteIdValido))
                     .ReturnsAsync(new Paciente { Id = PacienteIdValido, Nome = "Paciente Teste" });
        _medicoRepo.Setup(r => r.ObterPorIdAsync(MedicoIdValido))
                   .ReturnsAsync(new Medico { Id = MedicoIdValido, Nome = "Dr. Teste", Especialidade = "Clinico Geral" });
    }

    [Fact]
    public async Task CriarAsync_QuandoTudoValido_DeveAgendarConsulta()
    {
        ConfigurarPacienteEMedicoExistentes();
        _consultaRepo.Setup(r => r.ExisteConflitoAsync(MedicoIdValido, It.IsAny<DateTime>(), null))
                     .ReturnsAsync(false);
        _consultaRepo.Setup(r => r.CriarAsync(It.IsAny<Consulta>()))
                     .Callback<Consulta>(c => c.Id = ObjectId.GenerateNewId().ToString())
                     .Returns(Task.CompletedTask);

        var dto = new ConsultaCreateDto
        {
            PacienteId = PacienteIdValido,
            MedicoId = MedicoIdValido,
            DataHora = DateTime.UtcNow.AddDays(2),
            Observacoes = "Primeira consulta"
        };

        var resultado = await _service.CriarAsync(dto);

        Assert.NotNull(resultado);
        Assert.Equal("Paciente Teste", resultado.PacienteNome);
        Assert.Equal("Dr. Teste", resultado.MedicoNome);
        Assert.Equal(StatusConsulta.Agendada, resultado.Status);
        _consultaRepo.Verify(r => r.CriarAsync(It.IsAny<Consulta>()), Times.Once);
    }

    [Fact]
    public async Task ObterPorIdAsync_QuandoExiste_DeveRetornarComDadosDenormalizados()
    {
        var consultaId = ObjectId.GenerateNewId().ToString();
        var consulta = new Consulta
        {
            Id = consultaId,
            PacienteId = PacienteIdValido,
            MedicoId = MedicoIdValido,
            DataHora = DateTime.UtcNow.AddDays(1),
            Status = StatusConsulta.Agendada
        };
        _consultaRepo.Setup(r => r.ObterPorIdAsync(consultaId)).ReturnsAsync(consulta);
        ConfigurarPacienteEMedicoExistentes();

        var resultado = await _service.ObterPorIdAsync(consultaId);

        Assert.NotNull(resultado);
        Assert.Equal("Paciente Teste", resultado!.PacienteNome);
        Assert.Equal("Clinico Geral", resultado.MedicoEspecialidade);
    }

    [Fact]
    public async Task CriarAsync_QuandoDataNoPassado_DeveLancarInvalidOperation()
    {
        var dto = new ConsultaCreateDto
        {
            PacienteId = PacienteIdValido,
            MedicoId = MedicoIdValido,
            DataHora = DateTime.UtcNow.AddDays(-1),
            Observacoes = ""
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarAsync(dto));
        Assert.Contains("futura", ex.Message);
        _consultaRepo.Verify(r => r.CriarAsync(It.IsAny<Consulta>()), Times.Never);
    }

    [Fact]
    public async Task CriarAsync_QuandoPacienteNaoExiste_DeveLancarInvalidOperation()
    {
        _pacienteRepo.Setup(r => r.ObterPorIdAsync(PacienteIdValido)).ReturnsAsync((Paciente?)null);

        var dto = new ConsultaCreateDto
        {
            PacienteId = PacienteIdValido,
            MedicoId = MedicoIdValido,
            DataHora = DateTime.UtcNow.AddDays(1),
            Observacoes = ""
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarAsync(dto));
        Assert.Contains("Paciente", ex.Message);
    }

    [Fact]
    public async Task CriarAsync_QuandoExisteConflitoDeHorario_DeveLancarInvalidOperation()
    {
        ConfigurarPacienteEMedicoExistentes();
        _consultaRepo.Setup(r => r.ExisteConflitoAsync(MedicoIdValido, It.IsAny<DateTime>(), null))
                     .ReturnsAsync(true);

        var dto = new ConsultaCreateDto
        {
            PacienteId = PacienteIdValido,
            MedicoId = MedicoIdValido,
            DataHora = DateTime.UtcNow.AddDays(1),
            Observacoes = ""
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarAsync(dto));
        Assert.Contains("horario", ex.Message);
        _consultaRepo.Verify(r => r.CriarAsync(It.IsAny<Consulta>()), Times.Never);
    }

    [Fact]
    public async Task CriarAsync_QuandoPacienteIdInvalido_DeveLancarInvalidOperation()
    {
        var dto = new ConsultaCreateDto
        {
            PacienteId = "id-bagunçado",
            MedicoId = MedicoIdValido,
            DataHora = DateTime.UtcNow.AddDays(1),
            Observacoes = ""
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarAsync(dto));
    }
}
