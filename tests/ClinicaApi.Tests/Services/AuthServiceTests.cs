using ClinicaApi.DTOs;
using ClinicaApi.Models;
using ClinicaApi.Repositories;
using ClinicaApi.Services;
using Moq;
using Xunit;

namespace ClinicaApi.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUsuarioRepository> _repoMock = new();
    private readonly Mock<ITokenService> _tokenMock = new();
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _service = new AuthService(_repoMock.Object, _tokenMock.Object);
    }

    [Fact]
    public async Task RegistrarAsync_QuandoPrimeiroUsuario_DeveVirarAdmin()
    {
        var dto = new RegistroDto { Nome = "Boss", Email = "boss@teste.com", Senha = "senha123" };

        _repoMock.Setup(r => r.ObterPorEmailAsync(dto.Email)).ReturnsAsync((Usuario?)null);
        _repoMock.Setup(r => r.ContarAsync()).ReturnsAsync(0);
        _repoMock.Setup(r => r.CriarAsync(It.IsAny<Usuario>()))
                 .Callback<Usuario>(u => u.Id = "1")
                 .Returns(Task.CompletedTask);

        var resultado = await _service.RegistrarAsync(dto);

        Assert.Equal(PerfilUsuario.Admin, resultado.Perfil);
        _repoMock.Verify(r => r.CriarAsync(It.Is<Usuario>(u => u.Perfil == PerfilUsuario.Admin)), Times.Once);
    }

    [Fact]
    public async Task RegistrarAsync_QuandoJaExisteAlgumUsuario_NovoDeveVirarUsuarioComum()
    {
        var dto = new RegistroDto { Nome = "Comum", Email = "comum@teste.com", Senha = "senha123" };

        _repoMock.Setup(r => r.ObterPorEmailAsync(dto.Email)).ReturnsAsync((Usuario?)null);
        _repoMock.Setup(r => r.ContarAsync()).ReturnsAsync(3);
        _repoMock.Setup(r => r.CriarAsync(It.IsAny<Usuario>()))
                 .Callback<Usuario>(u => u.Id = "x")
                 .Returns(Task.CompletedTask);

        var resultado = await _service.RegistrarAsync(dto);

        Assert.Equal(PerfilUsuario.Usuario, resultado.Perfil);
    }

    [Fact]
    public async Task LoginAsync_QuandoCredenciaisValidas_DeveRetornarToken()
    {
        var senha = "minhaSenha123";
        var hash = BCrypt.Net.BCrypt.HashPassword(senha);
        var usuario = new Usuario
        {
            Id = "uid-1",
            Nome = "Fulano",
            Email = "fulano@teste.com",
            SenhaHash = hash,
            Perfil = PerfilUsuario.Admin
        };

        _repoMock.Setup(r => r.ObterPorEmailAsync(usuario.Email)).ReturnsAsync(usuario);
        var expira = DateTime.UtcNow.AddHours(2);
        _tokenMock.Setup(t => t.Gerar(usuario)).Returns(("token-falso", expira));

        var resultado = await _service.LoginAsync(new LoginDto { Email = usuario.Email, Senha = senha });

        Assert.Equal("token-falso", resultado.Token);
        Assert.Equal(expira, resultado.ExpiraEm);
        Assert.Equal(PerfilUsuario.Admin, resultado.Usuario.Perfil);
    }

    [Fact]
    public async Task RegistrarAsync_QuandoEmailJaCadastrado_DeveLancarInvalidOperation()
    {
        var dto = new RegistroDto { Nome = "Repetido", Email = "ja@teste.com", Senha = "senha123" };
        _repoMock.Setup(r => r.ObterPorEmailAsync(dto.Email))
                 .ReturnsAsync(new Usuario { Id = "outro", Email = dto.Email });

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RegistrarAsync(dto));
        Assert.Contains("email", ex.Message, StringComparison.OrdinalIgnoreCase);
        _repoMock.Verify(r => r.CriarAsync(It.IsAny<Usuario>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_QuandoSenhaIncorreta_DeveLancarInvalidOperation()
    {
        var hash = BCrypt.Net.BCrypt.HashPassword("senhaCerta");
        var usuario = new Usuario
        {
            Id = "uid-2",
            Email = "user@teste.com",
            SenhaHash = hash,
            Perfil = PerfilUsuario.Usuario
        };
        _repoMock.Setup(r => r.ObterPorEmailAsync(usuario.Email)).ReturnsAsync(usuario);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.LoginAsync(new LoginDto { Email = usuario.Email, Senha = "senhaErrada" }));

        _tokenMock.Verify(t => t.Gerar(It.IsAny<Usuario>()), Times.Never);
    }

    [Fact]
    public async Task PromoverAsync_QuandoUsuarioExisteEEhComum_DeveAtualizarParaAdmin()
    {
        var usuario = new Usuario { Id = "u1", Perfil = PerfilUsuario.Usuario };
        _repoMock.Setup(r => r.ObterPorIdAsync("u1")).ReturnsAsync(usuario);
        _repoMock.Setup(r => r.AtualizarPerfilAsync("u1", PerfilUsuario.Admin)).ReturnsAsync(true);

        var resultado = await _service.PromoverAsync("u1");

        Assert.True(resultado);
        _repoMock.Verify(r => r.AtualizarPerfilAsync("u1", PerfilUsuario.Admin), Times.Once);
    }

    [Fact]
    public async Task PromoverAsync_QuandoUsuarioNaoExiste_DeveRetornarFalse()
    {
        _repoMock.Setup(r => r.ObterPorIdAsync("inexistente")).ReturnsAsync((Usuario?)null);

        var resultado = await _service.PromoverAsync("inexistente");

        Assert.False(resultado);
        _repoMock.Verify(r => r.AtualizarPerfilAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}
