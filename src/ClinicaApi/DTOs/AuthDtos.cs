using System.ComponentModel.DataAnnotations;

namespace ClinicaApi.DTOs;

public class RegistroDto
{
    [Required]
    [StringLength(80, MinimumLength = 2)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter no minimo 6 caracteres.")]
    public string Senha { get; set; } = string.Empty;
}

public class LoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Senha { get; set; } = string.Empty;
}

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiraEm { get; set; }
    public UsuarioResponseDto Usuario { get; set; } = new();
}

public class UsuarioResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Perfil { get; set; } = string.Empty;
}
