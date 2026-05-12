# Clínica Médica - API REST + Frontend

Trabalho Prático Semestral da disciplina **Arquitetura de Aplicações Web (2026.1)**.

Aplicação web completa para gerenciamento de uma clínica médica, com API REST em **.NET 10**, persistência em **MongoDB** e frontend em **HTML + JavaScript** com navegação assíncrona.

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

## Status do projeto

- [x] Etapa 1 - Setup inicial (gitignore, docker-compose, README inicial)
- [x] Etapa 2 - Estrutura base da Web API
- [ ] Etapa 3 - CRUD de Pacientes
- [ ] Etapa 4 - CRUD de Médicos
- [ ] Etapa 5 - CRUD de Consultas
- [ ] Etapa 6 - Frontend HTML + JS
- [ ] Bônus A - JWT
- [ ] Bônus B - RBAC
- [ ] Bônus C - Testes unitários
- [ ] Bônus D - SOLID

## Autor

Mateus Morais Lopes
