using System.ComponentModel.DataAnnotations;
using ClinicaApi.Models;

namespace ClinicaApi.DTOs;

public class ConsultaCreateDto
{
    [Required]
    public string PacienteId { get; set; } = string.Empty;

    [Required]
    public string MedicoId { get; set; } = string.Empty;

    [Required]
    public DateTime DataHora { get; set; }

    [StringLength(500)]
    public string Observacoes { get; set; } = string.Empty;
}

public class ConsultaUpdateDto
{
    [Required]
    public DateTime DataHora { get; set; }

    [Required]
    public StatusConsulta Status { get; set; }

    [StringLength(500)]
    public string Observacoes { get; set; } = string.Empty;
}

public class ConsultaResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string PacienteId { get; set; } = string.Empty;
    public string PacienteNome { get; set; } = string.Empty;
    public string MedicoId { get; set; } = string.Empty;
    public string MedicoNome { get; set; } = string.Empty;
    public string MedicoEspecialidade { get; set; } = string.Empty;
    public DateTime DataHora { get; set; }
    public StatusConsulta Status { get; set; }
    public string Observacoes { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
}
