using System.ComponentModel.DataAnnotations;

namespace ClinicaApi.DTOs;

public class MedicoCreateDto
{
    [Required(ErrorMessage = "O nome e obrigatorio")]
    [StringLength(120, MinimumLength = 2)]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O CRM e obrigatorio")]
    [StringLength(20, MinimumLength = 4)]
    public string Crm { get; set; } = string.Empty;

    [Required(ErrorMessage = "A especialidade e obrigatoria")]
    [StringLength(80, MinimumLength = 3)]
    public string Especialidade { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string Telefone { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}

public class MedicoUpdateDto
{
    [Required]
    [StringLength(120, MinimumLength = 2)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [StringLength(80, MinimumLength = 3)]
    public string Especialidade { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string Telefone { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}

public class MedicoResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Crm { get; set; } = string.Empty;
    public string Especialidade { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
}
