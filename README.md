# TaskFlow 🚀

Gestor de proyectos y tareas tipo Trello/Notion, construido con **.NET 10** aplicando **Clean Architecture**, **CQRS**, **JWT Authentication** y **SignalR** para tiempo real.

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2025-CC2927?logo=microsoftsqlserver)](https://www.microsoft.com/sql-server)
[![SignalR](https://img.shields.io/badge/SignalR-Realtime-512BD4?logo=dotnet)](https://dotnet.microsoft.com/apps/aspnet/signalr)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

---

## 📋 Tabla de contenidos

- [Sobre el proyecto](#-sobre-el-proyecto)
- [Stack tecnológico](#-stack-tecnológico)
- [Arquitectura](#️-arquitectura)
- [Funcionalidades](#-funcionalidades)
- [Screenshots](#-screenshots)
- [Cómo correrlo](#-cómo-correrlo)
- [API Endpoints](#-api-endpoints)
- [Roadmap](#️-roadmap)
- [Autor](#-autor)

---

## 🎯 Sobre el proyecto

TaskFlow es una API REST que permite gestionar proyectos, columnas y tareas al estilo Kanban (Trello). El objetivo del proyecto es demostrar buenas prácticas de desarrollo backend con .NET:

- **Clean Architecture** con separación estricta de responsabilidades.
- **CQRS** con MediatR para casos de uso desacoplados.
- **FluentValidation** con pipeline automático.
- **JWT** para autenticación stateless.
- **BCrypt** para hashing seguro de contraseñas.
- **SignalR** para notificaciones en tiempo real.
- **Entity Framework Core** con configuraciones Fluent API.
- **Roles y permisos** por proyecto (Owner, Admin, Member).

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

### Próximamente
- **React + TypeScript** (frontend)
- **Docker** + **GitHub Actions**
- **Tests** con xUnit

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
└── TaskFlow.Api/
    ├── Endpoints/               → Auth, Projects, Columns, Tasks
    ├── Middleware/              → ExceptionHandlingMiddleware
    └── wwwroot/                 → live.html (cliente SignalR de prueba)
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
- [ ] **Fase 7:** Frontend React + TypeScript
- [ ] **Fase 8:** Tests con xUnit
- [ ] **Fase 9:** Docker + CI/CD + Deploy

---

## 👤 Autor

**Lalo Acevedo**
- GitHub: [@JacevedoEspindola](https://github.com/JacevedoEspindola)

---

## 📄 Licencia

Este proyecto está bajo la licencia MIT. Ver [`LICENSE`](LICENSE) para más detalles.
---

