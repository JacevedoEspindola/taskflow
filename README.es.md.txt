# TaskFlow 🚀

Un gestor de proyectos Kanban estilo Trello, construido con **.NET 10** (Clean Architecture) y **React 19** (TypeScript). Incluye actualizaciones en tiempo real, drag & drop, y está completamente deployado en la nube.

> 🌐 [English](README.md) | **Español**

### 🔗 Demo en Vivo
- **Frontend (App):** [https://taskflow-web-ecru.vercel.app](https://taskflow-web-ecru.vercel.app)
- **Backend (API):** [https://taskflow-api.wonderfulfield-f29193ab.westus2.azurecontainerapps.io/swagger](https://taskflow-api.wonderfulfield-f29193ab.westus2.azurecontainerapps.io/swagger)

**Credenciales de Prueba:**
- **Email:** `prubafront@prueba.com`
- **Password:** `Password123` *(o la que hayas usado)*

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-19-61DAFB?logo=react)](https://react.dev/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927?logo=microsoftsqlserver)](https://www.microsoft.com/sql-server)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker)](https://www.docker.com/)
[![Vercel](https://img.shields.io/badge/Vercel-Deployed-000000?logo=vercel)](https://vercel.com/)
[![Azure](https://img.shields.io/badge/Azure-Deployed-0078D4?logo=microsoftazure)](https://azure.microsoft.com/)
[![Tests](https://img.shields.io/badge/tests-27%20passed-success)](https://github.com/JacevedoEspindola/taskflow/actions)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

---

## 📋 Tabla de Contenidos

- [Sobre el Proyecto](#-sobre-el-proyecto)
- [Stack Tecnológico](#-stack-tecnológico)
- [Arquitectura](#️-arquitectura)
- [Funcionalidades](#-funcionalidades)
- [Testing](#-testing)
- [Screenshots](#-screenshots)
- [Cómo Correrlo](#-cómo-correrlo)
- [API Endpoints](#-api-endpoints)
- [Despliegue](#-despliegue)
- [Autor](#-autor)

---

## 🎯 Sobre el Proyecto

TaskFlow es un tablero Kanban full-stack (como Trello) que permite a los equipos gestionar proyectos, columnas y tareas en tiempo real.

El objetivo principal de este proyecto fue construir una **aplicación lista para producción** aplicando patrones de nivel empresarial y buenas prácticas, en lugar de un simple CRUD.

### Puntos Clave:
- **Clean Architecture & CQRS:** Separación de responsabilidades usando MediatR.
- **Colaboración en tiempo real:** Impulsado por SignalR (WebSockets).
- **Optimistic UI updates:** El drag & drop se siente instantáneo.
- **Cloud Native:** Backend dockerizado deployado en Azure, frontend en Vercel.
- **Test Driven:** 27 tests unitarios cubriendo la lógica de negocio crítica.

---

## 🛠️ Stack Tecnológico

### Backend
| Tecnología | Propósito |
|---|---|
| **.NET 10** | Framework principal |
| **ASP.NET Core Minimal APIs** | Endpoints HTTP |
| **Entity Framework Core 10** | ORM y Migraciones |
| **Azure SQL Database** | Base de datos relacional |
| **MediatR** | Patrón CQRS |
| **FluentValidation** | Validación de requests |
| **JWT & BCrypt** | Autenticación y hash de contraseñas |
| **SignalR** | WebSockets en tiempo real |
| **xUnit, Moq, FluentAssertions** | Tests unitarios |
| **Docker & Azure Container Apps** | Hosting y Despliegue |
| **GitHub Actions** | Pipeline CI/CD |

### Frontend
| Tecnología | Propósito |
|---|---|
| **React 19 & TypeScript** | Librería de UI |
| **Vite** | Build tool |
| **Tailwind CSS** | Estilos |
| **React Router** | Ruteo |
| **TanStack Query** | Fetching de datos y caché |
| **Zustand** | Manejo de estado global |
| **React Hook Form & Zod** | Formularios y validación |
| **@dnd-kit** | Funcionalidad drag & drop |
| **@microsoft/signalr** | Cliente en tiempo real |
| **Vercel** | Hosting y Despliegue |

---

## 🏛️ Arquitectura

El backend sigue **Clean Architecture**, con las dependencias apuntando hacia el centro:

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

### Estructura de Carpetas
```
TaskFlow/
├── TaskFlow.Domain/              → Entidades (User, Project, Task, etc.)
├── TaskFlow.Application/         → Handlers CQRS, Validadores, Interfaces
├── TaskFlow.Infrastructure/      → EF Core, Auth, SignalR, Repositorios
├── TaskFlow.Api/                 → Minimal APIs, Middlewares, DI
├── tests/TaskFlow.UnitTests/     → 27 Tests unitarios
└── docker-compose.yml            → Configuración de infraestructura local
```

---

## ✅ Funcionalidades

### Autenticación
- [x] Registro e inicio de sesión con JWT.
- [x] Hash seguro de contraseñas con BCrypt.
- [x] Middleware de manejo global de excepciones.
- [x] Validación automática de requests con FluentValidation.

### Proyectos y Roles
- [x] CRUD completo de Proyectos.
- [x] Control de acceso basado en roles (Owner, Admin, Member).
- [x] Solo los miembros pueden ver/editar proyectos específicos.
- [x] Columnas por defecto autogeneradas (To Do, In Progress, Done).

### Columnas y Tareas
- [x] CRUD completo de Columnas y Tareas.
- [x] Prioridades de tareas (Low, Medium, High, Critical) y fechas límite.
- [x] Mover tareas entre columnas (Drag & Drop).
- [x] Asignar tareas a miembros del proyecto.

### Tiempo Real y UX
- [x] **SignalR:** Actualizaciones instantáneas entre múltiples pestañas.
- [x] **Optimistic UI:** Feedback instantáneo en drag & drop (sin estados de carga).
- [x] UI moderna y responsive construida con Tailwind CSS.

---

## 🧪 Testing

El backend incluye **27 tests unitarios** cubriendo validadores, handlers de auth, creación de proyectos y la lógica compleja de mover tareas.

```bash
dotnet test
```

- **Framework:** xUnit
- **Mocking:** Moq
- **Assertions:** FluentAssertions
- **In-Memory DB:** EF Core InMemory

---

## 📸 Screenshots

### Frontend (Tablero Kanban)
![Frontend Board](docs/screenshots/frontend-board.png)

### Swagger UI (API)
![Swagger](docs/Swagger.PNG)

### Tiempo Real (SignalR en acción)
![SignalR](docs/signalr/01-live-events.png)

### Usuarios en Base de Datos (Contraseñas Hasheadas)
![SSMS](docs/ssms-users.PNG)

---

## 🚀 Cómo Correrlo

### Requisitos Previos
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js](https://nodejs.org/) (v20+)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Opción 1: Correr con Docker (Backend)
```bash
git clone https://github.com/JacevedoEspindola/taskflow.git
cd taskflow
docker-compose up --build
```
La API estará disponible en `http://localhost:8080/swagger`.

### Opción 2: Correr Localmente (Backend)
1. Configurá tu cadena de conexión en `TaskFlow.Api/appsettings.json`.
2. Aplicá las migraciones:
   ```bash
   dotnet ef database update -p TaskFlow.Infrastructure -s TaskFlow.Api
   ```
3. Corré la API:
   ```bash
   dotnet run --project TaskFlow.Api
   ```

### Correr el Frontend
```bash
# Clonar el repo del frontend (link abajo)
git clone https://github.com/JacevedoEspindola/taskflow-web.git
cd taskflow-web
npm install
npm run dev
```
El frontend estará disponible en `http://localhost:5173`.

---

## 📡 API Endpoints

### Auth
- `POST /api/auth/register` — Registrar un nuevo usuario.
- `POST /api/auth/login` — Iniciar sesión y recibir un JWT.

### Proyectos
- `GET /api/projects` — Listar proyectos del usuario.
- `POST /api/projects` — Crear un proyecto.
- `GET /api/projects/{id}` — Obtener detalles del proyecto.
- `PUT /api/projects/{id}` — Actualizar proyecto.
- `DELETE /api/projects/{id}` — Eliminar proyecto.

### Columnas y Tareas
- `GET /api/projects/{projectId}/columns` — Listar columnas.
- `POST /api/columns/{columnId}/tasks` — Crear una tarea.
- `PATCH /api/tasks/{id}/move` — Mover tarea a otra columna.
- `PATCH /api/tasks/{id}/assign` — Asignar tarea a un usuario.

### SignalR
- `/hubs/notifications` — Hub de WebSockets para actualizaciones en tiempo real.

---

## ☁️ Despliegue y CI/CD

- **Backend:** Deployado en **Azure Container Apps** usando un Dockerfile multi-stage. Las imágenes se suben a **Azure Container Registry (ACR)**.
- **Base de Datos:** Alojada en **Azure SQL Database** (tier Serverless para optimizar costos).
- **Frontend:** Deployado en **Vercel** con despliegues automáticos desde la rama `main`.
- **CI/CD:** **GitHub Actions** corre los tests en cada push y asegura que la imagen Docker se construya correctamente.

> ⚠️ **Aviso de Cold Start:** Como este es un proyecto de portfolio usando tiers gratuitos/serverless, la API y la base de datos pueden tardar **30-40 segundos** en responder en la primera request después de un período largo de inactividad. Las siguientes requests son rápidas.

---

## 🔗 Repositorios Relacionados

- **Repositorio del Frontend:** [taskflow-web](https://github.com/JacevedoEspindola/taskflow-web)

---

## 👤 Autor

**Jose Acevedo**
- GitHub: [@JacevedoEspindola](https://github.com/JacevedoEspindola)

---

## 📄 Licencia

Este proyecto está bajo la licencia MIT. Ver el archivo [`LICENSE`](LICENSE) para más detalles.