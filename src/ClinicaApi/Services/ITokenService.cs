using ClinicaApi.Models;

namespace ClinicaApi.Services;

public interface ITokenService
{
    (string Token, DateTime ExpiraEm) Gerar(Usuario usuario);
}
