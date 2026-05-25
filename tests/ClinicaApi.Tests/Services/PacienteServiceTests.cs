using ClinicaApi.DTOs;
using ClinicaApi.Models;
using ClinicaApi.Repositories;
using ClinicaApi.Services;
using Moq;
using Xunit;

namespace ClinicaApi.Tests.Services;

public class PacienteServiceTests
{
    private readonly Mock<IPacienteRepository> _repoMock;
    private readonly PacienteService _service;

    public PacienteServiceTests()
    {
        _repoMock = new Mock<IPacienteRepository>();
        _service = new PacienteService(_repoMock.Object);
    }

    [Fact]
    public async Task CriarAsync_QuandoCpfNovo_DeveCriarERetornarPaciente()
    {
        var dto = new PacienteCreateDto
        {
            Nome = "Joao Silva",
            Cpf = "12345678900",
            DataNascimento = new DateTime(1990, 5, 10),
            Telefone = "+5531999990000",
            Email = "joao@teste.com"
        };

        _repoMock.Setup(r => r.ObterPorCpfAsync(dto.Cpf)).ReturnsAsync((Paciente?)null);
        _repoMock.Setup(r => r.CriarAsync(It.IsAny<Paciente>()))
                 .Callback<Paciente>(p => p.Id = "abc123")
                 .Returns(Task.CompletedTask);

        var resultado = await _service.CriarAsync(dto);

        Assert.NotNull(resultado);
        Assert.Equal("Joao Silva", resultado.Nome);
        Assert.Equal("12345678900", resultado.Cpf);
        _repoMock.Verify(r => r.CriarAsync(It.IsAny<Paciente>()), Times.Once);
    }

    [Fact]
    public async Task ListarAsync_QuandoHaPacientes_DeveRetornarLista()
    {
        var pacientes = new List<Paciente>
        {
            new() { Id = "1", Nome = "A", Cpf = "11111111111" },
            new() { Id = "2", Nome = "B", Cpf = "22222222222" }
        };
        _repoMock.Setup(r => r.ListarAsync()).ReturnsAsync(pacientes);

        var resultado = await _service.ListarAsync();

        Assert.Equal(2, resultado.Count);
        Assert.Equal("A", resultado[0].Nome);
        Assert.Equal("B", resultado[1].Nome);
    }

    [Fact]
    public async Task CriarAsync_QuandoCpfJaExiste_DeveLancarInvalidOperation()
    {
        var dto = new PacienteCreateDto
        {
            Nome = "Joao",
            Cpf = "12345678900",
            DataNascimento = new DateTime(1990, 1, 1),
            Telefone = "+5531999990000",
            Email = "joao@teste.com"
        };
        _repoMock.Setup(r => r.ObterPorCpfAsync(dto.Cpf))
                 .ReturnsAsync(new Paciente { Id = "existente", Cpf = dto.Cpf });

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarAsync(dto));
        Assert.Contains("CPF", ex.Message);
        _repoMock.Verify(r => r.CriarAsync(It.IsAny<Paciente>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_QuandoIdNaoExiste_DeveRetornarFalse()
    {
        var dto = new PacienteUpdateDto
        {
            Nome = "Novo Nome",
            Telefone = "+5531988887777",
            Email = "novo@teste.com",
            DataNascimento = new DateTime(1990, 1, 1)
        };
        _repoMock.Setup(r => r.AtualizarAsync(It.IsAny<string>(), It.IsAny<Paciente>()))
                 .ReturnsAsync(false);

        var resultado = await _service.AtualizarAsync("id-inexistente", dto);

        Assert.False(resultado);
    }
}
