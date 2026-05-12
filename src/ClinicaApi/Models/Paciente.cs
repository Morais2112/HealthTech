using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ClinicaApi.Models;

public class Paciente
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("nome")]
    public string Nome { get; set; } = string.Empty;

    [BsonElement("cpf")]
    public string Cpf { get; set; } = string.Empty;

    [BsonElement("dataNascimento")]
    public DateTime DataNascimento { get; set; }

    [BsonElement("telefone")]
    public string Telefone { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("criadoEm")]
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
