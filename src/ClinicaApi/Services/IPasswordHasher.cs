namespace ClinicaApi.Services;

public interface IPasswordHasher
{
    string Hash(string senhaPura);
    bool Verificar(string senhaPura, string hashArmazenado);
}
