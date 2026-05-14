using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ClinicaApi.Models;

public class Medico
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("nome")]
    public string Nome { get; set; } = string.Empty;

    [BsonElement("crm")]
    public string Crm { get; set; } = string.Empty;

    [BsonElement("especialidade")]
    public string Especialidade { get; set; } = string.Empty;

    [BsonElement("telefone")]
    public string Telefone { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("criadoEm")]
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
