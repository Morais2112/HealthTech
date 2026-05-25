# Health Tech - API REST + Frontend

Trabalho Prático Semestral da disciplina **Arquitetura de Aplicações Web (2026.1)**.

Aplicação web completa para gerenciamento da clínica médica **Health Tech**, com API REST em **.NET 10**, persistência em **MongoDB** e frontend em **HTML + JavaScript** com navegação assíncrona.

## Domínio

Três entidades principais relacionadas:

- **Paciente** - cadastro dos pacientes da clínica.
- **Médico** - cadastro dos profissionais, com especialidade e CRM.
- **Consulta** - agendamento que relaciona um paciente a um médico, em uma data/hora.

## Stack

| Camada    | Tecnologia                        |
|-----------|-----------------------------------|
| Backend   | .NET 10 (C#) - ASP.NET Core Web API |
| Banco     | MongoDB 7                         |
| Frontend  | HTML + JavaScript (fetch)         |
| Docs API  | Swagger / OpenAPI                 |
| Container | Docker Compose (MongoDB)          |

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (para subir o MongoDB)
- [Git](https://git-scm.com/)

## Como executar (em construção)

> Este README será atualizado a cada etapa do projeto. Por enquanto, apenas o ambiente base está configurado.

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

### 3. Rodar a API (em breve)

A partir da Etapa 2.

### 4. Acessar o Swagger (em breve)

A partir da Etapa 2: `http://localhost:5000/swagger`.

## Variáveis de Ambiente

Veja `.env.example` para a lista completa. As principais são:

| Variável                  | Descrição                                  |
|---------------------------|--------------------------------------------|
| `MONGO_CONNECTION_STRING` | String de conexão usada pela API           |
| `MONGO_DATABASE_NAME`     | Nome do banco no Mongo                     |
| `JWT_SECRET`              | Chave para assinar tokens JWT (Bônus A)    |

## Testes unitários (Bônus C)

O projeto `tests/ClinicaApi.Tests` usa **xUnit** + **Moq** para testar a camada de Services em isolamento, com os repositórios mockados.

```bash
dotnet test
```

Cobertura:

| Service          | Cenários cobertos                                                                                                                |
|------------------|----------------------------------------------------------------------------------------------------------------------------------|
| `PacienteService`| Criar com CPF novo, listar, criar com CPF duplicado (erro), atualizar id inexistente                                            |
| `ConsultaService`| Agendar consulta válida, obter por id (com dados denormalizados), data no passado (erro), paciente inexistente (erro), conflito de horário (erro), ObjectId inválido (erro) |
| `AuthService`    | Primeiro usuário vira Admin, segundo vira Usuario, login válido, email duplicado (erro), senha incorreta (erro), promoção de usuário, promoção de id inexistente |

## RBAC (Bônus B)

Há dois perfis no sistema:

| Perfil    | Permissões                                                          |
|-----------|----------------------------------------------------------------------|
| `Usuario` | Listar / criar / editar Pacientes, Médicos e Consultas              |
| `Admin`   | Tudo do Usuario + **DELETE** + gerenciamento de usuários (`/auth/usuarios`) |

**Bootstrap:** o **primeiro usuário registrado no sistema vira Admin automaticamente**. A partir dele, qualquer outro Admin pode promover/rebaixar usuários em `Usuários` no menu.

> Se você já registrou um usuário antes desta etapa e ele está como `Usuario`, você pode promovê-lo manualmente no MongoDB (Mongo Express → `usuarios` → editar campo `Perfil` para `"Admin"`).

## Status do projeto

- [x] Etapa 1 - Setup inicial (gitignore, docker-compose, README inicial)
- [x] Etapa 2 - Estrutura base da Web API
- [x] Etapa 3 - CRUD de Pacientes
- [x] Etapa 4 - CRUD de Médicos
- [x] Etapa 5 - CRUD de Consultas
- [x] Etapa 6 - Frontend HTML + JS
- [x] Bônus A - JWT
- [x] Bônus B - RBAC (perfis Admin/Usuario, DELETE restrito, primeiro usuário vira Admin)
- [x] Bônus C - Testes unitários (xUnit + Moq, cobrindo Paciente / Consulta / Auth Services)
- [ ] Bônus D - SOLID

## Autor

Mateus Morais Lopes
