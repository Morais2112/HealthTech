# Aplicação dos Princípios SOLID — Health Tech

Este documento descreve como **4 dos 5 princípios SOLID** foram aplicados na arquitetura do projeto (Bônus D).

## S — Single Responsibility Principle (Responsabilidade Única)

Cada classe tem uma responsabilidade clara e isolada. A API foi organizada em três camadas, e cada uma só sabe da sua parte:

| Camada       | Responsabilidade                                          | Exemplo                                                      |
|--------------|-----------------------------------------------------------|--------------------------------------------------------------|
| `Controllers`| Receber requisições HTTP, validar entrada, devolver resposta | `PacientesController` apenas roteia e formata `IActionResult`|
| `Services`   | Conter as regras de negócio                                | `ConsultaService` valida `ObjectId`, data futura, conflitos  |
| `Repositories` | Acessar o MongoDB (queries, índices, mapeamento)         | `UsuarioRepository` cria o índice único de email             |
| `DTOs`       | Definir o contrato de entrada/saída da API                 | `PacienteCreateDto` separado de `PacienteUpdateDto`          |
| `Models`     | Representar entidades do domínio                           | `Paciente`, `Medico`, `Consulta`, `Usuario`                  |

Refatoração adicional motivada por SRP:

- `AuthService` antes acumulava **autenticação** (registro/login) **e** gestão de perfis (promover/rebaixar). Foi dividido em **`AuthService`** (apenas registro e login) e **`UsuarioService`** (gestão de perfis), porque são responsabilidades distintas.

## O — Open/Closed Principle (Aberto/Fechado)

As classes estão **abertas para extensão e fechadas para modificação** graças ao uso de interfaces.

Exemplo concreto: o hashing de senhas era feito chamando `BCrypt.Net.BCrypt.HashPassword(...)` direto dentro do `AuthService`. Se a clínica decidisse migrar para Argon2 ou PBKDF2, seria preciso editar o `AuthService`. Foi extraída a abstração `IPasswordHasher`:

```csharp
public interface IPasswordHasher
{
    string Hash(string senhaPura);
    bool Verificar(string senhaPura, string hashArmazenado);
}
```

A implementação atual é `BcryptPasswordHasher`. Trocar por `Argon2PasswordHasher` no futuro exige **apenas registrar a nova implementação no `Program.cs`** — o `AuthService` permanece intacto.

Outro exemplo: cada CRUD segue o mesmo padrão (Repository → Service → Controller), então adicionar uma nova entidade não modifica as existentes — extende o sistema com novas classes.

## I — Interface Segregation Principle (Segregação de Interfaces)

Os clientes não são forçados a depender de métodos que não usam.

Exemplo concreto da refatoração:

- Antes, **`IAuthService`** crescia para 5 métodos: `Registrar`, `Login`, `ListarUsuarios`, `Promover`, `Rebaixar`. Um `AuthController` que só faz login não devia ter de "enxergar" `Promover`.
- Foi separado em duas interfaces coesas:
  - `IAuthService` — `Registrar`, `Login` (autenticação)
  - `IUsuarioService` — `Listar`, `Promover`, `Rebaixar` (gestão de perfis)

Os repositórios também respeitam o princípio: cada um expõe apenas os métodos da sua entidade (`IPacienteRepository` não tem `ObterPorCrmAsync`, por exemplo).

## D — Dependency Inversion Principle (Inversão de Dependência)

Módulos de alto nível (Services, Controllers) **não dependem de implementações concretas**, e sim de abstrações (interfaces). A composição é feita pelo container de DI do ASP.NET Core no `Program.cs`.

```csharp
builder.Services.AddScoped<IPacienteRepository, PacienteRepository>();
builder.Services.AddScoped<IPacienteService, PacienteService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();
```

Benefícios práticos observados:

- **Testabilidade.** Os testes unitários do Bônus C mockam `IPacienteRepository`, `IUsuarioRepository`, `IPasswordHasher` e `ITokenService` com **Moq**, sem subir Mongo, sem JWT real, sem custo do BCrypt.
- **Substituibilidade.** Como `AuthService` recebe `IPasswordHasher` pelo construtor, os testes injetam um mock que devolve `"hash-fake"` instantaneamente.

## L — Liskov Substitution Principle (não enfatizado)

O LSP foi respeitado de forma trivial — toda implementação concreta (`PacienteRepository`, `BcryptPasswordHasher` etc.) honra integralmente o contrato da sua interface, sem lançar exceções inesperadas nem reduzir o domínio aceito. Não foi feita uma refatoração dedicada porque a hierarquia do projeto é rasa (interface → uma implementação), então o princípio não aparece como ponto de risco.

## Resumo

| Princípio | Aplicado? | Onde olhar                                                                     |
|-----------|-----------|--------------------------------------------------------------------------------|
| **S**RP   | Sim       | Separação `Controllers` / `Services` / `Repositories` e split Auth/Usuario     |
| **O**CP   | Sim       | `IPasswordHasher` permite trocar BCrypt sem alterar `AuthService`              |
| **L**SP   | Implícito | Hierarquias rasas; toda implementação respeita o contrato                      |
| **I**SP   | Sim       | `IAuthService` separado de `IUsuarioService`; repositórios coesos por entidade |
| **D**IP   | Sim       | DI registra abstrações no `Program.cs`; serviços dependem só de interfaces      |
