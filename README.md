# HealthTech — Clinic Management REST API

> API REST completa para gerenciamento de clínica médica, construída com **.NET 10 (C#)**, **MongoDB** e autenticação **JWT + RBAC**.

![.NET](https://img.shields.io/badge/.NET_10-512BD4?style=flat&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=flat&logo=csharp&logoColor=white)
![MongoDB](https://img.shields.io/badge/MongoDB-47A248?style=flat&logo=mongodb&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?style=flat&logo=docker&logoColor=white)
![JWT](https://img.shields.io/badge/JWT-000000?style=flat&logo=json-web-tokens&logoColor=FF007F)
![xUnit](https://img.shields.io/badge/xUnit-tested-brightgreen?style=flat)

---

## Sobre o projeto

O **HealthTech** é uma API RESTful para gerenciamento de clínicas médicas, cobrindo as entidades **Pacientes**, **Médicos** e **Consultas**. O sistema conta com autenticação segura via JWT, controle de acesso baseado em papéis (RBAC) e testes unitários na camada de serviços.

O projeto também inclui um **frontend HTML/JS** que consome a API de forma assíncrona, demonstrando a integração end-to-end da aplicação.

### Funcionalidades

- Cadastro e gerenciamento de Pacientes, Médicos e Consultas (CRUD completo)
- Autenticação com JWT (HMAC SHA-256) e hashing de senha com BCrypt
- RBAC com dois perfis: `Usuario` (leitura/escrita) e `Admin` (acesso total + gestão de usuários)
- Bootstrap automático: o primeiro usuário registrado recebe o perfil `Admin`
- Documentação interativa via Swagger/OpenAPI
- Testes unitários com xUnit + Moq na camada de Services
- Containerização do banco de dados com Docker Compose

---

## Stack

| Camada     | Tecnologia                          |
|------------|-------------------------------------|
| Backend    | .NET 10 (C#) — ASP.NET Core Web API |
| Banco      | MongoDB 7                           |
| Auth       | JWT (HMAC SHA-256) + BCrypt         |
| Testes     | xUnit + Moq                         |
| Docs       | Swagger / OpenAPI                   |
| Frontend   | HTML + JavaScript (Fetch API)       |
| Container  | Docker Compose                      |

---

## Arquitetura

```
HealthTech/
├── src/
│   └── ClinicaApi/
│       ├── Controllers/       # Endpoints HTTP (recebem e respondem requisições)
│       ├── Services/          # Lógica de negócio
│       ├── Repositories/      # Acesso ao MongoDB
│       ├── Models/            # Entidades do domínio
│       └── Program.cs         # Composição de DI e middlewares
├── tests/
│   └── ClinicaApi.Tests/      # Testes unitários (xUnit + Moq)
├── frontend/                  # Interface HTML/JS
├── docs/
│   └── SOLID.md               # Documentação das decisões de design
└── docker-compose.yml
```

A separação em camadas segue os princípios **SOLID** — detalhados em [`docs/SOLID.md`](./docs/SOLID.md):

- **SRP**: Controllers, Services e Repositories com responsabilidades bem definidas
- **OCP**: `IPasswordHasher` abstrai o algoritmo de hashing, permitindo troca sem alterar `AuthService`
- **ISP**: `IAuthService` (autenticação) separado de `IUsuarioService` (gestão de perfis)
- **DIP**: dependências injetadas via interface; composição feita no `Program.cs`

---

## Como executar

### Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Git](https://git-scm.com/)

### 1. Clonar o repositório

```bash
git clone https://github.com/Morais2112/HealthTech.git
cd HealthTech
```

### 2. Configurar variáveis de ambiente

```bash
cp .env.example .env
# Edite o .env se quiser alterar credenciais ou a chave JWT
```

### 3. Subir o MongoDB

```bash
docker compose up -d
```

Serviços disponíveis:
- MongoDB: `mongodb://localhost:27017`
- Mongo Express (UI): `http://localhost:8081`

### 4. Rodar a API

```bash
cd src/ClinicaApi
dotnet run
```

### 5. Acessar

| Interface      | URL                              |
|----------------|----------------------------------|
| Frontend       | http://localhost:5000            |
| Swagger UI     | http://localhost:5000/swagger    |
| Health Check   | http://localhost:5000/api/health |

> **Primeiro acesso:** cadastre o primeiro usuário — ele recebe o perfil `Admin` automaticamente.

---

## Endpoints

### Auth

| Método | Rota                            | Acesso      |
|--------|---------------------------------|-------------|
| POST   | `/auth/registrar`               | Público     |
| POST   | `/auth/login`                   | Público     |
| GET    | `/auth/usuarios`                | Admin       |
| POST   | `/auth/usuarios/{id}/promover`  | Admin       |
| POST   | `/auth/usuarios/{id}/rebaixar`  | Admin       |

### Pacientes / Médicos / Consultas

| Método | Rota              | Acesso      |
|--------|-------------------|-------------|
| GET    | `/{recurso}`      | Autenticado |
| POST   | `/{recurso}`      | Autenticado |
| PUT    | `/{recurso}/{id}` | Autenticado |
| DELETE | `/{recurso}/{id}` | Admin       |

---

## Testes

```bash
dotnet test
```

Cobertura por camada de serviço:

| Service           | Cenários                                                                                                              |
|-------------------|-----------------------------------------------------------------------------------------------------------------------|
| `PacienteService` | Criar com CPF novo, listar, CPF duplicado (erro), atualizar id inexistente                                            |
| `ConsultaService` | Consulta válida, dados denormalizados, data no passado (erro), paciente inexistente (erro), conflito de horário (erro), ObjectId inválido (erro) |
| `AuthService`     | Primeiro usuário → Admin, segundo → Usuario, login válido, email duplicado (erro), senha incorreta (erro)             |
| `UsuarioService`  | Listar, promover, promover quem já é Admin (no-op), rebaixar, operações com id inexistente                            |

---

## Variáveis de ambiente

| Variável                  | Descrição                                        |
|---------------------------|--------------------------------------------------|
| `MONGO_CONNECTION_STRING` | String de conexão com o MongoDB                  |
| `MONGO_DATABASE_NAME`     | Nome do banco de dados                           |
| `JWT_SECRET`              | Chave de assinatura dos tokens (mín. 32 chars)   |
| `JWT_ISSUER`              | Emissor do token                                 |
| `JWT_AUDIENCE`            | Audiência do token                               |
| `JWT_EXPIRES_MINUTES`     | Tempo de expiração (em minutos)                  |

---

## Autor

**Mateus Morais Lopes**
[![LinkedIn](https://img.shields.io/badge/LinkedIn-0077B5?style=flat&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/mateus-morais-lopes/)
[![GitHub](https://img.shields.io/badge/GitHub-181717?style=flat&logo=github&logoColor=white)](https://github.com/Morais2112)
