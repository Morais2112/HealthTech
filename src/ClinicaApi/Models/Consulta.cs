using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ClinicaApi.Models;

public class Consulta
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("pacienteId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string PacienteId { get; set; } = string.Empty;

    [BsonElement("medicoId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string MedicoId { get; set; } = string.Empty;

    [BsonElement("dataHora")]
    public DateTime DataHora { get; set; }

    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    public StatusConsulta Status { get; set; } = StatusConsulta.Agendada;

    [BsonElement("observacoes")]
    public string Observacoes { get; set; } = string.Empty;

    [BsonElement("criadoEm")]
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
