# TaskFlow 🚀

Gestor de proyectos y tareas tipo Trello/Notion, construido con **.NET 10** aplicando **Clean Architecture**, **CQRS**, **JWT Authentication** y **SignalR** para tiempo real.

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2025-CC2927?logo=microsoftsqlserver)](https://www.microsoft.com/sql-server)
[![SignalR](https://img.shields.io/badge/SignalR-Realtime-512BD4?logo=dotnet)](https://dotnet.microsoft.com/apps/aspnet/signalr)
[![Tests](https://img.shields.io/badge/tests-27%20passed-success)](https://github.com/JacevedoEspindola/taskflow)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

---

## 📋 Tabla de contenidos

- [Sobre el proyecto](#-sobre-el-proyecto)
- [Stack tecnológico](#-stack-tecnológico)
- [Arquitectura](#️-arquitectura)
- [Funcionalidades](#-funcionalidades)
- [Testing](#-testing)
- [Screenshots](#-screenshots)
- [Cómo correrlo](#-cómo-correrlo)
- [API Endpoints](#-api-endpoints)
- [Roadmap](#️-roadmap)
- [Autor](#-autor)

---

## 🎯 Sobre el proyecto

TaskFlow is a REST API that allows managing projects, columns and tasks in a Kanban style (like Trello). The goal of this project is to demonstrate backend best practices with .NET:

- **Clean Architecture** with strict separation of concerns.
- **CQRS** with MediatR for decoupled use cases.
- **FluentValidation** with automatic pipeline.
- **JWT** for stateless authentication.
- **BCrypt** for secure password hashing.
- **SignalR** for real-time notifications.
- **Entity Framework Core** with Fluent API configurations.
- **Role-based permissions** per project (Owner, Admin, Member).
- **Unit tests** with xUnit, Moq and FluentAssertions.

---

## 🛠️ Stack tecnológico

### Backend
| Tecnología | Uso |
|---|---|
| **.NET 10** | Framework principal |
| **ASP.NET Core Minimal APIs** | Endpoints HTTP |
| **Entity Framework Core 10** | ORM |
| **SQL Server 2025** | Base de datos |
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

### Próximamente
- **React + TypeScript** (frontend)
- **Docker** + **GitHub Actions**
- **Deploy** en Azure / Railway

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
    └── TaskFlow.UnitTests/      → xUnit tests (27 tests)
```

---

## ✅ Funcionalidades

### Autenticación
- [x] Registro de usuarios con validación
- [x] Login con generación de JWT
- [x] Hash de contraseñas con BCrypt
- [x] Validación automática con FluentValidation
- [x] Manejo global de excepciones con respuestas JSON

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

El proyecto incluye **27 tests unitarios** con xUnit, Moq y FluentAssertions, cubriendo los casos de uso principales.

### Ejecutar los tests

```bash
dotnet test
```

### Cobertura de tests

| Módulo | Tests |
|---|---|
| **Auth** | Validadores de Register/Login, `RegisterCommandHandler`, `LoginCommandHandler` |
| **Projects** | `CreateProjectCommandHandler` (creación + validaciones) |
| **Columns** | `CreateColumnCommandHandler` (posición automática + permisos) |
| **Tasks** | `MoveTaskCommandHandler` (reordenamiento, notificaciones SignalR, validaciones) |

### Ejemplo de test

```csharp
[Fact]
public async Task Should_Move_Task_To_Target_Column_When_Target_Is_Different()
{
    // Arrange
    var (db, _, _, inProgressColumnId, taskId) = await SeedScenarioAsync();
    var handler = new MoveTaskCommandHandler(db, _currentUserMock.Object, _notifierMock.Object);
    var command = new MoveTaskCommand(taskId, inProgressColumnId, Position: null);

    // Act
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert
    result.ColumnId.Should().Be(inProgressColumnId);
    result.ColumnName.Should().Be("In Progress");

    var task = await db.TaskItems.FindAsync(taskId);
    task!.Position.Should().Be(1);
}
```

### Resultado

![Tests](docs/tests/01-test-results.png)

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
- [SQL Server Express o superior](https://www.microsoft.com/sql-server/sql-server-downloads)
- [SQL Server Management Studio (SSMS)](https://aka.ms/ssmsfullsetup) *(opcional)*

### Pasos

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
| `POST` | `/api/auth/register` | Register user |
| `POST` | `/api/auth/login` | Login (returns JWT) |

### Projects
| Método | Ruta | Descripción |
|---|---|---|
| `GET` | `/api/projects` | List my projects |
| `POST` | `/api/projects` | Create project |
| `GET` | `/api/projects/{id}` | Get project details |
| `PUT` | `/api/projects/{id}` | Update (Owner/Admin) |
| `DELETE` | `/api/projects/{id}` | Delete (Owner/Admin) |

### Columns
| Método | Ruta | Descripción |
|---|---|---|
| `GET` | `/api/projects/{projectId}/columns` | List columns |
| `POST` | `/api/projects/{projectId}/columns` | Create column |
| `PUT` | `/api/projects/{projectId}/columns/{id}` | Update / Reorder |
| `DELETE` | `/api/projects/{projectId}/columns/{id}` | Delete |

### Tasks
| Método | Ruta | Descripción |
|---|---|---|
| `GET` | `/api/columns/{columnId}/tasks` | Tasks in a column |
| `POST` | `/api/columns/{columnId}/tasks` | Create task |
| `GET` | `/api/tasks/{id}` | Task details |
| `PUT` | `/api/tasks/{id}` | Update task |
| `PATCH` | `/api/tasks/{id}/move` | **Move between columns** |
| `PATCH` | `/api/tasks/{id}/assign` | **Assign user** |
| `DELETE` | `/api/tasks/{id}` | Delete task |

### SignalR
| Ruta | Descripción |
|---|---|
| `/hubs/notifications` | Real-time hub (requires JWT) |

**Events emitted:**
- `TaskCreated` — when a task is created
- `TaskMoved` — when a task is moved between columns
- `TaskAssigned` — when the assignee changes

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
- [x] **Fase 2:** Authentication (Register + Login + JWT)
- [x] **Fase 3:** Projects CRUD + Roles + Authorization
- [x] **Fase 4:** Columns CRUD
- [x] **Fase 5:** Tasks CRUD + Move + Assign
- [x] **Fase 6:** SignalR (real-time)
- [x] **Fase 7:** Unit tests with xUnit + Moq
- [ ] **Fase 8:** Docker + CI/CD + Deploy
- [ ] **Fase 9:** Frontend with React + TypeScript

---

## 👤 Autor

**Jose Acevedo**
- GitHub: [@JacevedoEspindola](https://github.com/JacevedoEspindola)

---

## 📄 Licencia

Este proyecto está bajo la licencia MIT. Ver [`LICENSE`](LICENSE) para más detalles.