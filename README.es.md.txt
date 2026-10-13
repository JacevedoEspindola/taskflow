# TaskFlow 🚀

> 🌐 [English](README.md) | **Español**

Gestor de proyectos y tareas tipo Trello/Notion, construido con **.NET 10** aplicando **Clean Architecture**, **CQRS**, **JWT Authentication**, **SignalR** para tiempo real, y totalmente **Dockerizado**.

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927?logo=microsoftsqlserver)](https://www.microsoft.com/sql-server)
[![SignalR](https://img.shields.io/badge/SignalR-Realtime-512BD4?logo=dotnet)](https://dotnet.microsoft.com/apps/aspnet/signalr)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker)](https://www.docker.com/)
[![Tests](https://img.shields.io/badge/tests-27%20passed-success)](https://github.com/JacevedoEspindola/taskflow)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

---

## 📋 Tabla de contenidos

- [Sobre el proyecto](#-sobre-el-proyecto)
- [Stack tecnológico](#️-stack-tecnológico)
- [Arquitectura](#️-arquitectura)
- [Funcionalidades](#-funcionalidades)
- [Testing](#-testing)
- [Docker](#-docker)
- [Screenshots](#-screenshots)
- [Cómo correrlo](#-cómo-correrlo)
- [API Endpoints](#-api-endpoints)
- [Roadmap](#️-roadmap)
- [Autor](#-autor)

---

## 🎯 Sobre el proyecto

TaskFlow es una API REST que permite gestionar proyectos, columnas y tareas al estilo Kanban (como Trello). El objetivo del proyecto es demostrar buenas prácticas de desarrollo backend con .NET:

- **Clean Architecture** con separación estricta de responsabilidades.
- **CQRS** con MediatR para casos de uso desacoplados.
- **FluentValidation** con pipeline automático.
- **JWT** para autenticación stateless.
- **BCrypt** para hashing seguro de contraseñas.
- **SignalR** para notificaciones en tiempo real.
- **Entity Framework Core** con configuraciones Fluent API.
- **Roles y permisos** por proyecto (Owner, Admin, Member).
- **Tests unitarios** con xUnit, Moq y FluentAssertions.
- **Totalmente Dockerizado** con multi-stage builds y docker-compose.

---

## 🛠️ Stack tecnológico

### Backend
| Tecnología | Uso |
|---|---|
| **.NET 10** | Framework principal |
| **ASP.NET Core Minimal APIs** | Endpoints HTTP |
| **Entity Framework Core 10** | ORM |
| **SQL Server 2022** | Base de datos |
| **MediatR** | Patrón CQRS |
| **FluentValidation** | Validación de comandos |
| **JWT** | Autenticación |
| **BCrypt.Net-Next** | Hash de contraseñas |
| **SignalR** | Tiempo real (WebSockets) |
| **Swagger / OpenAPI** | Documentación de la API |

### Testing
| Tecnología | Uso |
|---|---|
| **xUnit** | Framework de testing |
| **Moq** | Mocking de dependencias |
| **FluentAssertions** | Assertions legibles |
| **EF Core InMemory** | Base de datos en memoria |
| **Coverlet** | Cobertura de código |

### DevOps
| Tecnología | Uso |
|---|---|
| **Docker** | Contenedores |
| **Docker Compose** | Orquestación multi-contenedor |

### Próximamente
- **React + TypeScript** (frontend)
- **GitHub Actions** (CI/CD)
- **Deploy en la nube** (Azure / Railway)

---

## 🏛️ Arquitectura

El proyecto sigue **Clean Architecture**, con las dependencias apuntando siempre hacia el centro:

```
┌─────────────────────────────────────────┐
│              TaskFlow.Api               │  ← Endpoints, DI, Middlewares, Hubs
├─────────────────────────────────────────┤
│         TaskFlow.Infrastructure         │  ← EF Core, JWT, BCrypt, SignalR
├─────────────────────────────────────────┤
│          TaskFlow.Application           │  ← Casos de uso, MediatR, DTOs
├─────────────────────────────────────────┤
│             TaskFlow.Domain             │  ← Entidades, reglas de negocio
└─────────────────────────────────────────┘
```

### Estructura de carpetas

```
TaskFlow/
├── TaskFlow.Domain/
│   └── Entities/                → User, Project, ProjectMember, BoardColumn, TaskItem
│
├── TaskFlow.Application/
│   ├── Common/
│   │   ├── Behaviors/           → ValidationBehavior (FluentValidation)
│   │   └── Interfaces/          → IApplicationDbContext, IJwtService,
│   │                              IPasswordHasher, ICurrentUserService, IRealTimeNotifier
│   └── Features/
│       ├── Auth/                → Register + Login
│       ├── Projects/            → CRUD de proyectos
│       ├── Columns/             → CRUD de columnas
│       └── Tasks/               → CRUD de tareas + Move + Assign
│
├── TaskFlow.Infrastructure/
│   ├── Authentication/          → JwtService, PasswordHasher, JwtSettings
│   ├── Persistence/             → ApplicationDbContext + Configurations
│   ├── RealTime/                → NotificationHub, SignalRNotifier
│   └── Services/                → CurrentUserService
│
├── TaskFlow.Api/
│   ├── Endpoints/               → Auth, Projects, Columns, Tasks
│   ├── Middleware/              → ExceptionHandlingMiddleware
│   └── wwwroot/                 → live.html (cliente SignalR de prueba)
│
└── tests/
    └── TaskFlow.UnitTests/      → tests con xUnit (27 tests)
```

---

## ✅ Funcionalidades

### Autenticación
- [x] Registro de usuarios con validación
- [x] Login con generación de JWT
- [x] Hash de contraseñas con BCrypt
- [x] Validación automática con FluentValidation
- [x] Manejo global de excepciones con respuestas JSON estructuradas

### Proyectos
- [x] CRUD completo de proyectos
- [x] Autorización por roles (Owner, Admin, Member)
- [x] Columnas por defecto (To Do, In Progress, Done) al crear
- [x] Solo miembros del proyecto pueden verlo/editar

### Columnas
- [x] CRUD completo de columnas
- [x] Posicionamiento y reordenamiento
- [x] Solo Owners/Admins pueden modificar

### Tareas
- [x] CRUD completo de tareas
- [x] Prioridades (Low, Medium, High, Critical)
- [x] Fecha límite opcional
- [x] **Mover tareas entre columnas** (drag & drop backend)
- [x] **Asignar tareas a miembros** del proyecto
- [x] Posicionamiento dentro de columnas

### Tiempo real (SignalR)
- [x] Notificaciones al crear/mover/asignar tareas
- [x] Grupos por proyecto (solo miembros reciben eventos)
- [x] Autenticación JWT en el hub
- [x] Cliente HTML de prueba (`/live.html`)

---

## 🧪 Testing

El proyecto incluye **27 tests unitarios** con xUnit, Moq y FluentAssertions, cubriendo la lógica de negocio crítica.

### Ejecutar los tests

```bash
dotnet test
```

### Cobertura

| Módulo | Tests |
|---|---|
| **Auth** | Validadores de Register/Login, `RegisterCommandHandler`, `LoginCommandHandler` |
| **Projects** | `CreateProjectCommandHandler` |
| **Columns** | `CreateColumnCommandHandler` |
| **Tasks** | `MoveTaskCommandHandler` (reordenamiento, SignalR, validaciones) |

### Test de ejemplo

```csharp
[Fact]
public async Task Should_Move_Task_To_Target_Column_When_Target_Is_Different()
{
    var (db, _, _, inProgressColumnId, taskId) = await SeedScenarioAsync();
    var handler = new MoveTaskCommandHandler(db, _currentUserMock.Object, _notifierMock.Object);
    var command = new MoveTaskCommand(taskId, inProgressColumnId, Position: null);

    var result = await handler.Handle(command, CancellationToken.None);

    result.ColumnId.Should().Be(inProgressColumnId);
    result.ColumnName.Should().Be("In Progress");
}
```

### Resultado

![Tests](docs/tests/01-test-results.png)

---

## 🐳 Docker

El proyecto incluye **Dockerfile multi-stage** y **docker-compose** para correr todo el stack (API + SQL Server) con un solo comando.

### Levantar todo

```bash
docker-compose up --build
```

La API queda disponible en `http://localhost:8080/swagger`.

### Qué incluye

- **Dockerfile multi-stage** → imagen final ~200 MB (vs ~1 GB con SDK).
- **SQL Server 2022** en contenedor con health check.
- **Auto-migración** al arrancar (solo en Development).
- **Volumen persistente** para datos de SQL Server.

### Docker Compose up

![Docker Compose](docs/docker/01-docker-compose-up.png)

### Swagger corriendo en Docker

![Swagger in Docker](docs/docker/03-swagger-in-docker.png)

---

## 📸 Screenshots

### Swagger UI
![Swagger](docs/Swagger.PNG)

### Autenticación

**Register exitoso:**
![Register](docs/register.PNG)

**Login exitoso:**
![Login](docs/02-login-success.PNG)

**Usuarios en la base de datos (con password hasheado):**
![SSMS](docs/ssms-users.PNG)

### CRUD de Proyectos

**Crear proyecto (con columnas automáticas):**
![Create Project](docs/04-create-project.PNG)

**Listar proyectos:**
![List Projects](docs/05-list-projects.PNG)

**Detalle de proyecto:**
![Project Detail](docs/06-project-detail.PNG)

### Tiempo Real (SignalR)

**Eventos en vivo cuando se crean/mueven/asignan tareas:**
![SignalR Live](docs/signalr/01-live-events.png)

### Estructura del proyecto
![Estructura](docs/structure.PNG)

---

## 🚀 Cómo correrlo

### Requisitos previos
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server Express o superior](https://www.microsoft.com/sql-server/sql-server-downloads) *(o usa Docker)*
- [SQL Server Management Studio (SSMS)](https://aka.ms/ssmsfullsetup) *(opcional)*
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) *(opcional, recomendado)*

### Opción 1: Con Docker (recomendado)

```bash
git clone https://github.com/JacevedoEspindola/taskflow.git
cd taskflow
docker-compose up --build
```

Abre `http://localhost:8080/swagger`.

### Opción 2: Localmente

1. **Clonar el repositorio:**
   ```bash
   git clone https://github.com/JacevedoEspindola/taskflow.git
   cd taskflow
   ```

2. **Configurar la cadena de conexión** en `TaskFlow.Api/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=TaskFlowDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
   }
   ```

3. **Configurar el JWT Secret** en `TaskFlow.Api/appsettings.json`:
   ```json
   "Jwt": {
     "Secret": "TaskFlow-Super-Secret-Key-For-Development-Only-Min-32-Chars-Long!!",
     "Issuer": "TaskFlowAPI",
     "Audience": "TaskFlowClient",
     "ExpirationMinutes": 60
   }
   ```

4. **Aplicar migraciones:**
   ```bash
   dotnet ef database update -p TaskFlow.Infrastructure -s TaskFlow.Api
   ```

5. **Correr la API:**
   ```bash
   dotnet run --project TaskFlow.Api
   ```

6. **Abrir Swagger:**
   ```
   https://localhost:7236/swagger
   ```

---

## 📡 API Endpoints

### Auth
| Método | Ruta | Descripción |
|---|---|---|
| `POST` | `/api/auth/register` | Registrar usuario |
| `POST` | `/api/auth/login` | Login (devuelve JWT) |

### Projects
| Método | Ruta | Descripción |
|---|---|---|
| `GET` | `/api/projects` | Listar mis proyectos |
| `POST` | `/api/projects` | Crear proyecto |
| `GET` | `/api/projects/{id}` | Ver detalle |
| `PUT` | `/api/projects/{id}` | Editar (Owner/Admin) |
| `DELETE` | `/api/projects/{id}` | Eliminar (Owner/Admin) |

### Columns
| Método | Ruta | Descripción |
|---|---|---|
| `GET` | `/api/projects/{projectId}/columns` | Listar columnas |
| `POST` | `/api/projects/{projectId}/columns` | Crear columna |
| `PUT` | `/api/projects/{projectId}/columns/{id}` | Editar / Reordenar |
| `DELETE` | `/api/projects/{projectId}/columns/{id}` | Eliminar |

### Tasks
| Método | Ruta | Descripción |
|---|---|---|
| `GET` | `/api/columns/{columnId}/tasks` | Tareas de una columna |
| `POST` | `/api/columns/{columnId}/tasks` | Crear tarea |
| `GET` | `/api/tasks/{id}` | Detalle de tarea |
| `PUT` | `/api/tasks/{id}` | Editar tarea |
| `PATCH` | `/api/tasks/{id}/move` | **Mover entre columnas** |
| `PATCH` | `/api/tasks/{id}/assign` | **Asignar usuario** |
| `DELETE` | `/api/tasks/{id}` | Eliminar tarea |

### SignalR
| Ruta | Descripción |
|---|---|
| `/hubs/notifications` | Hub de tiempo real (requiere JWT) |

**Eventos emitidos:**
- `TaskCreated` — al crear una tarea
- `TaskMoved` — al mover una tarea entre columnas
- `TaskAssigned` — al asignar/cambiar el asignado

### Probar la autenticación

**Registro:**
```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "user@test.com",
  "password": "Password123",
  "fullName": "Test User"
}
```

**Login:**
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@test.com",
  "password": "Password123"
}
```

---

## 🗺️ Roadmap

- [x] **Fase 1:** Setup + Clean Architecture
- [x] **Fase 2:** Autenticación (Register + Login + JWT)
- [x] **Fase 3:** CRUD de Proyectos + Roles + Autorización
- [x] **Fase 4:** CRUD de Columnas
- [x] **Fase 5:** CRUD de Tareas + Move + Assign
- [x] **Fase 6:** SignalR (tiempo real)
- [x] **Fase 7:** Tests unitarios con xUnit + Moq
- [x] **Fase 8:** Docker + Docker Compose
- [ ] **Fase 9:** CI/CD con GitHub Actions
- [ ] **Fase 10:** Deploy en la nube (Azure / Railway)
- [ ] **Fase 11:** Frontend con React + TypeScript

---

## 👤 Autor

**Lalo Acevedo**
- GitHub: [@JacevedoEspindola](https://github.com/JacevedoEspindola)

---

## 📄 Licencia

Este proyecto está bajo la licencia MIT. Ver [`LICENSE`](LICENSE) para más detalles.