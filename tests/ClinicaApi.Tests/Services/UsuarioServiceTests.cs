using ClinicaApi.Models;
using ClinicaApi.Repositories;
using ClinicaApi.Services;
using Moq;
using Xunit;

namespace ClinicaApi.Tests.Services;

public class UsuarioServiceTests
{
    private readonly Mock<IUsuarioRepository> _repoMock = new();
    private readonly UsuarioService _service;

    public UsuarioServiceTests()
    {
        _service = new UsuarioService(_repoMock.Object);
    }

    [Fact]
    public async Task ListarAsync_DeveRetornarTodosOsUsuarios()
    {
        var usuarios = new List<Usuario>
        {
            new() { Id = "1", Nome = "A", Email = "a@x.com", Perfil = PerfilUsuario.Admin },
            new() { Id = "2", Nome = "B", Email = "b@x.com", Perfil = PerfilUsuario.Usuario }
        };
        _repoMock.Setup(r => r.ListarAsync()).ReturnsAsync(usuarios);

        var resultado = (await _service.ListarAsync()).ToList();

        Assert.Equal(2, resultado.Count);
        Assert.Equal("a@x.com", resultado[0].Email);
        Assert.Equal(PerfilUsuario.Usuario, resultado[1].Perfil);
    }

    [Fact]
    public async Task PromoverAsync_QuandoUsuarioComum_DeveAtualizarParaAdmin()
    {
        var usuario = new Usuario { Id = "u1", Perfil = PerfilUsuario.Usuario };
        _repoMock.Setup(r => r.ObterPorIdAsync("u1")).ReturnsAsync(usuario);
        _repoMock.Setup(r => r.AtualizarPerfilAsync("u1", PerfilUsuario.Admin)).ReturnsAsync(true);

        var resultado = await _service.PromoverAsync("u1");

        Assert.True(resultado);
        _repoMock.Verify(r => r.AtualizarPerfilAsync("u1", PerfilUsuario.Admin), Times.Once);
    }

    [Fact]
    public async Task PromoverAsync_QuandoUsuarioJaEhAdmin_NaoDeveTocarNoBanco()
    {
        var usuario = new Usuario { Id = "u1", Perfil = PerfilUsuario.Admin };
        _repoMock.Setup(r => r.ObterPorIdAsync("u1")).ReturnsAsync(usuario);

        var resultado = await _service.PromoverAsync("u1");

        Assert.True(resultado);
        _repoMock.Verify(r => r.AtualizarPerfilAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task PromoverAsync_QuandoUsuarioNaoExiste_DeveRetornarFalse()
    {
        _repoMock.Setup(r => r.ObterPorIdAsync("inexistente")).ReturnsAsync((Usuario?)null);

        var resultado = await _service.PromoverAsync("inexistente");

        Assert.False(resultado);
        _repoMock.Verify(r => r.AtualizarPerfilAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task RebaixarAsync_QuandoAdmin_DeveAtualizarParaUsuario()
    {
        var usuario = new Usuario { Id = "u2", Perfil = PerfilUsuario.Admin };
        _repoMock.Setup(r => r.ObterPorIdAsync("u2")).ReturnsAsync(usuario);
        _repoMock.Setup(r => r.AtualizarPerfilAsync("u2", PerfilUsuario.Usuario)).ReturnsAsync(true);

        var resultado = await _service.RebaixarAsync("u2");

        Assert.True(resultado);
        _repoMock.Verify(r => r.AtualizarPerfilAsync("u2", PerfilUsuario.Usuario), Times.Once);
    }

    [Fact]
    public async Task RebaixarAsync_QuandoNaoExiste_DeveRetornarFalse()
    {
        _repoMock.Setup(r => r.ObterPorIdAsync("x")).ReturnsAsync((Usuario?)null);

        var resultado = await _service.RebaixarAsync("x");

        Assert.False(resultado);
    }
}
