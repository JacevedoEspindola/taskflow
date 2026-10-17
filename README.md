# TaskFlow 🚀

A full-stack, Trello-style Kanban project manager built with **.NET 10** (Clean Architecture) and **React 19** (TypeScript). Features real-time updates, drag & drop, and is fully deployed to the cloud.

> 🌐 **English** | [Español](README.es.md)

### 🔗 Live Demo
- **Frontend (App):** [https://taskflow-web-ecru.vercel.app](https://taskflow-web-ecru.vercel.app)
- **Backend (API):** [https://taskflow-api.wonderfulfield-f29193ab.westus2.azurecontainerapps.io/swagger](https://taskflow-api.wonderfulfield-f29193ab.westus2.azurecontainerapps.io/swagger)

**Test Credentials:**
- **Email:** `prubafront@prueba.com`
- **Password:** `Password123` *(or whatever password you used)*

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-19-61DAFB?logo=react)](https://react.dev/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927?logo=microsoftsqlserver)](https://www.microsoft.com/sql-server)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker)](https://www.docker.com/)
[![Vercel](https://img.shields.io/badge/Vercel-Deployed-000000?logo=vercel)](https://vercel.com/)
[![Azure](https://img.shields.io/badge/Azure-Deployed-0078D4?logo=microsoftazure)](https://azure.microsoft.com/)
[![Tests](https://img.shields.io/badge/tests-27%20passed-success)](https://github.com/JacevedoEspindola/taskflow/actions)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

---

## 📋 Table of Contents

- [About the Project](#-about-the-project)
- [Tech Stack](#-tech-stack)
- [Architecture](#️-architecture)
- [Features](#-features)
- [Testing](#-testing)
- [Screenshots](#-screenshots)
- [Getting Started](#-getting-started)
- [API Endpoints](#-api-endpoints)
- [Deployment](#-deployment)
- [Author](#-author)

---

## 🎯 About the Project

TaskFlow is a full-stack Kanban board (like Trello) that allows teams to manage projects, columns, and tasks in real time. 

The main goal of this project was to build a **production-ready application** applying enterprise-level patterns and best practices, rather than just a simple CRUD.

### Key Highlights:
- **Clean Architecture & CQRS:** Separation of concerns using MediatR.
- **Real-time collaboration:** Powered by SignalR (WebSockets).
- **Optimistic UI updates:** Drag & drop feels instantaneous.
- **Cloud Native:** Fully Dockerized backend deployed on Azure, frontend on Vercel.
- **Test Driven:** 27 unit tests covering the critical business logic.

---

## 🛠️ Tech Stack

### Backend
| Technology | Purpose |
|---|---|
| **.NET 10** | Core framework |
| **ASP.NET Core Minimal APIs** | HTTP endpoints |
| **Entity Framework Core 10** | ORM & Migrations |
| **Azure SQL Database** | Relational database |
| **MediatR** | CQRS pattern |
| **FluentValidation** | Request validation |
| **JWT & BCrypt** | Authentication & Password hashing |
| **SignalR** | Real-time WebSockets |
| **xUnit, Moq, FluentAssertions** | Unit testing |
| **Docker & Azure Container Apps** | Hosting & Deployment |
| **GitHub Actions** | CI/CD pipeline |

### Frontend
| Technology | Purpose |
|---|---|
| **React 19 & TypeScript** | UI Library |
| **Vite** | Build tool |
| **Tailwind CSS** | Styling |
| **React Router** | Routing |
| **TanStack Query** | Data fetching & caching |
| **Zustand** | Global state management |
| **React Hook Form & Zod** | Forms & validation |
| **@dnd-kit** | Drag & drop functionality |
| **@microsoft/signalr** | Real-time client |
| **Vercel** | Hosting & Deployment |

---

## 🏛️ Architecture

The backend follows **Clean Architecture**, with dependencies pointing inward:

```
┌─────────────────────────────────────────┐
│              TaskFlow.Api               │  ← Endpoints, DI, Middlewares, Hubs
├─────────────────────────────────────────┤
│         TaskFlow.Infrastructure         │  ← EF Core, JWT, BCrypt, SignalR
├─────────────────────────────────────────┤
│          TaskFlow.Application           │  ← Use cases, MediatR, DTOs
├─────────────────────────────────────────┤
│             TaskFlow.Domain             │  ← Entities, business rules
└─────────────────────────────────────────┘
```

### Folder Structure
```
TaskFlow/
├── TaskFlow.Domain/              → Entities (User, Project, Task, etc.)
├── TaskFlow.Application/         → CQRS Handlers, Validators, Interfaces
├── TaskFlow.Infrastructure/      → EF Core, Auth, SignalR, Repositories
├── TaskFlow.Api/                 → Minimal APIs, Middlewares, DI
├── tests/TaskFlow.UnitTests/     → 27 Unit tests
└── docker-compose.yml            → Local infrastructure setup
```

---

## ✅ Features

### Authentication
- [x] User registration & login with JWT.
- [x] Secure password hashing with BCrypt.
- [x] Global exception handling middleware.
- [x] Automatic request validation with FluentValidation.

### Projects & Roles
- [x] Full CRUD for Projects.
- [x] Role-based access control (Owner, Admin, Member).
- [x] Only members can view/edit specific projects.
- [x] Auto-generated default columns (To Do, In Progress, Done).

### Columns & Tasks
- [x] Full CRUD for Columns and Tasks.
- [x] Task priorities (Low, Medium, High, Critical) and due dates.
- [x] Move tasks between columns (Drag & Drop).
- [x] Assign tasks to project members.

### Real-Time & UX
- [x] **SignalR:** Instant updates across multiple browser tabs.
- [x] **Optimistic UI:** Instant drag & drop feedback (no loading state).
- [x] Responsive, modern UI built with Tailwind CSS.

---

## 🧪 Testing

The backend includes **27 unit tests** covering validators, auth handlers, project creation, and complex task-moving logic.

```bash
dotnet test
```

- **Framework:** xUnit
- **Mocking:** Moq
- **Assertions:** FluentAssertions
- **In-Memory DB:** EF Core InMemory

---

## 📸 Screenshots

### Frontend (Kanban Board)
![Frontend Board](docs/screenshots/frontend-board.png)

### Swagger UI (API)
![Swagger](docs/Swagger.PNG)

### Real-Time (SignalR in action)
![SignalR](docs/signalr/01-live-events.png)

### Users in Database (Hashed Passwords)
![SSMS](docs/ssms-users.PNG)

---

## 🚀 Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js](https://nodejs.org/) (v20+)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Option 1: Run with Docker (Backend)
```bash
git clone https://github.com/JacevedoEspindola/taskflow.git
cd taskflow
docker-compose up --build
```
API will be available at `http://localhost:8080/swagger`.

### Option 2: Run Locally (Backend)
1. Configure your SQL Server connection string in `TaskFlow.Api/appsettings.json`.
2. Apply migrations:
   ```bash
   dotnet ef database update -p TaskFlow.Infrastructure -s TaskFlow.Api
   ```
3. Run the API:
   ```bash
   dotnet run --project TaskFlow.Api
   ```

### Run Frontend
```bash
# Clone the frontend repo (link below)
git clone https://github.com/JacevedoEspindola/taskflow-web.git
cd taskflow-web
npm install
npm run dev
```
Frontend will be available at `http://localhost:5173`.

---

## 📡 API Endpoints

### Auth
- `POST /api/auth/register` — Register a new user.
- `POST /api/auth/login` — Login and receive a JWT.

### Projects
- `GET /api/projects` — List user's projects.
- `POST /api/projects` — Create a project.
- `GET /api/projects/{id}` — Get project details.
- `PUT /api/projects/{id}` — Update project.
- `DELETE /api/projects/{id}` — Delete project.

### Columns & Tasks
- `GET /api/projects/{projectId}/columns` — List columns.
- `POST /api/columns/{columnId}/tasks` — Create a task.
- `PATCH /api/tasks/{id}/move` — Move task to another column.
- `PATCH /api/tasks/{id}/assign` — Assign task to a user.

### SignalR
- `/hubs/notifications` — WebSocket hub for real-time updates.

---

## ☁️ Deployment & CI/CD

- **Backend:** Deployed to **Azure Container Apps** using a multi-stage Dockerfile. Images are pushed to **Azure Container Registry (ACR)**.
- **Database:** Hosted on **Azure SQL Database** (Serverless tier to optimize costs).
- **Frontend:** Deployed to **Vercel** with automatic deployments from the `main` branch.
- **CI/CD:** **GitHub Actions** runs tests on every push and ensures the Docker image builds successfully.

> ⚠️ **Cold Start Notice:** Because this is a portfolio project using free/serverless tiers, the API and Database may take **30-40 seconds** to respond on the very first request after a long period of inactivity. Subsequent requests are fast.

---

## 🔗 Related Repositories

- **Frontend Repository:** [taskflow-web](https://github.com/JacevedoEspindola/taskflow-web)

---

## 👤 Author

**Jose Acevedo**
- GitHub: [@JacevedoEspindola](https://github.com/JacevedoEspindola)

---

## 📄 License

This project is licensed under the MIT License. See the [`LICENSE`](LICENSE) file for details.