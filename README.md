# Health Tech - API REST + Frontend

Trabalho Prático Semestral da disciplina **Arquitetura de Aplicações Web (2026.1)**.

Aplicação web completa para gerenciamento da clínica médica **Health Tech**, com API REST em **.NET 10**, persistência em **MongoDB** e frontend em **HTML + JavaScript** com navegação assíncrona.

## Domínio

Três entidades principais relacionadas:

- **Paciente** - cadastro dos pacientes da clínica.
- **Médico** - cadastro dos profissionais, com especialidade e CRM.
- **Consulta** - agendamento que relaciona um paciente a um médico, em uma data/hora.

## Stack

| Camada    | Tecnologia                          |
|-----------|-------------------------------------|
| Backend   | .NET 10 (C#) - ASP.NET Core Web API |
| Banco     | MongoDB 7                           |
| Frontend  | HTML + JavaScript (fetch)           |
| Docs API  | Swagger / OpenAPI                   |
| Auth      | JWT (HMAC SHA256) + BCrypt          |
| Testes    | xUnit + Moq                         |
| Container | Docker Compose (MongoDB)            |

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (para subir o MongoDB)
- [Git](https://git-scm.com/)

## Como executar

### 1. Clonar o repositório

```bash
git clone <url-do-repo>
cd clinica_CRUD
```

### 2. Subir o MongoDB

Copie o arquivo de exemplo e ajuste se quiser:

```bash
cp .env.example .env
```

Suba o banco:

```bash
docker compose up -d
```

Serviços disponíveis:

- MongoDB em `mongodb://localhost:27017`
- Mongo Express (UI web) em `http://localhost:8081` (usuário/senha do `.env`)

### 3. Rodar a API

```bash
cd src/ClinicaApi
dotnet run
```

A API sobe em `http://localhost:5000` e também serve o frontend estático.

### 4. Acessar a aplicação

- Frontend: `http://localhost:5000`
- Swagger: `http://localhost:5000/swagger`
- Health check: `http://localhost:5000/api/health`

### 5. Primeiro acesso

1. Abra o frontend e clique em **Criar conta**.
2. Cadastre o primeiro usuário — ele vira **Admin** automaticamente (regra do bootstrap).
3. Faça login e use o sistema.

## Variáveis de Ambiente

Veja `.env.example` para a lista completa. As principais são:

| Variável                  | Descrição                                       |
|---------------------------|-------------------------------------------------|
| `MONGO_CONNECTION_STRING` | String de conexão usada pela API                |
| `MONGO_DATABASE_NAME`     | Nome do banco no Mongo                          |
| `JWT_SECRET`              | Chave para assinar tokens JWT (mínimo 32 chars) |
| `JWT_ISSUER`              | Emissor do token                                |
| `JWT_AUDIENCE`            | Audiência do token                              |
| `JWT_EXPIRES_MINUTES`     | Tempo de expiração do token                     |

## Endpoints principais

| Verbo  | Rota                                | Autorização          |
|--------|-------------------------------------|----------------------|
| POST   | `/auth/registrar`                   | Anônimo              |
| POST   | `/auth/login`                       | Anônimo              |
| GET    | `/auth/usuarios`                    | Admin                |
| POST   | `/auth/usuarios/{id}/promover`      | Admin                |
| POST   | `/auth/usuarios/{id}/rebaixar`      | Admin                |
| GET    | `/pacientes`                        | Autenticado          |
| POST   | `/pacientes`                        | Autenticado          |
| PUT    | `/pacientes/{id}`                   | Autenticado          |
| DELETE | `/pacientes/{id}`                   | **Admin**            |
| GET    | `/medicos`                          | Autenticado          |
| POST   | `/medicos`                          | Autenticado          |
| PUT    | `/medicos/{id}`                     | Autenticado          |
| DELETE | `/medicos/{id}`                     | **Admin**            |
| GET    | `/consultas`                        | Autenticado          |
| POST   | `/consultas`                        | Autenticado          |
| PUT    | `/consultas/{id}`                   | Autenticado          |
| DELETE | `/consultas/{id}`                   | **Admin**            |

## RBAC (Bônus B)

Dois perfis no sistema:

| Perfil    | Permissões                                                                  |
|-----------|------------------------------------------------------------------------------|
| `Usuario` | Listar / criar / editar Pacientes, Médicos e Consultas                       |
| `Admin`   | Tudo do Usuario + **DELETE** + gerenciamento de usuários (`/auth/usuarios`)  |

**Bootstrap:** o **primeiro usuário registrado vira Admin automaticamente**. Depois, qualquer Admin pode promover/rebaixar usuários na tela **Usuários** do frontend.

> Se quiser zerar o banco para reiniciar do zero: `docker compose down -v && docker compose up -d`.

## Testes unitários (Bônus C)

O projeto `tests/ClinicaApi.Tests` usa **xUnit** + **Moq** para testar a camada de Services em isolamento, com os repositórios e dependências externas mockadas.

```bash
dotnet test
```

Cobertura:

| Service           | Cenários cobertos                                                                                                                |
|-------------------|----------------------------------------------------------------------------------------------------------------------------------|
| `PacienteService` | Criar com CPF novo, listar, criar com CPF duplicado (erro), atualizar id inexistente                                            |
| `ConsultaService` | Agendar consulta válida, obter por id (com dados denormalizados), data no passado (erro), paciente inexistente (erro), conflito de horário (erro), ObjectId inválido (erro) |
| `AuthService`     | Primeiro usuário vira Admin, segundo vira Usuario, login válido, email duplicado (erro), senha incorreta (erro)                  |
| `UsuarioService`  | Listar, promover comum a admin, promover quem já é admin (no-op), promover inexistente, rebaixar admin, rebaixar inexistente     |

## SOLID (Bônus D)

A documentação completa está em [`docs/SOLID.md`](docs/SOLID.md). Em resumo:

- **SRP**: separação `Controllers` / `Services` / `Repositories`, e `AuthService` dividido em autenticação (`AuthService`) e gestão de perfis (`UsuarioService`).
- **OCP**: `IPasswordHasher` permite trocar BCrypt por outro algoritmo sem editar `AuthService`.
- **ISP**: `IAuthService` (registrar/login) separado de `IUsuarioService` (listar/promover/rebaixar).
- **DIP**: serviços e controllers dependem de abstrações; container de DI faz a composição no `Program.cs`.

## Status do projeto

- [x] Etapa 1 - Setup inicial (gitignore, docker-compose, README inicial)
- [x] Etapa 2 - Estrutura base da Web API
- [x] Etapa 3 - CRUD de Pacientes
- [x] Etapa 4 - CRUD de Médicos
- [x] Etapa 5 - CRUD de Consultas
- [x] Etapa 6 - Frontend HTML + JS
- [x] Bônus A - JWT
- [x] Bônus B - RBAC (perfis Admin/Usuario, DELETE restrito, primeiro usuário vira Admin)
- [x] Bônus C - Testes unitários (xUnit + Moq)
- [x] Bônus D - SOLID (SRP, OCP, ISP, DIP — ver `docs/SOLID.md`)

## Autor

Mateus Morais Lopes
