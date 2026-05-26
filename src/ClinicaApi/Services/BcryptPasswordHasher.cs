namespace ClinicaApi.Services;

public class BcryptPasswordHasher : IPasswordHasher
{
    public string Hash(string senhaPura) => BCrypt.Net.BCrypt.HashPassword(senhaPura);

    public bool Verificar(string senhaPura, string hashArmazenado) =>
        BCrypt.Net.BCrypt.Verify(senhaPura, hashArmazenado);
}
