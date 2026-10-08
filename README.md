# TaskFlow 🚀

Gestor de proyectos y tareas tipo Trello/Notion, construido con **.NET 10** aplicando **Clean Architecture**, **CQRS** y **JWT Authentication**.

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2025-CC2927?logo=microsoftsqlserver)](https://www.microsoft.com/sql-server)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

---

## 📋 Tabla de contenidos

- [Sobre el proyecto](#-sobre-el-proyecto)
- [Stack tecnológico](#-stack-tecnológico)
- [Arquitectura](#️-arquitectura)
- [Funcionalidades](#-funcionalidades)
- [Screenshots](#-screenshots)
- [Cómo correrlo](#-cómo-correrlo)
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
- **Entity Framework Core** con configuraciones Fluent API.

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
| **Swagger / OpenAPI** | Documentación de la API |

### Próximamente
- **SignalR** (actualizaciones en tiempo real)
- **React + TypeScript** (frontend)
- **Docker** + **GitHub Actions**
- **Tests** con xUnit

---

## 🏛️ Arquitectura

El proyecto sigue **Clean Architecture**, con las dependencias apuntando siempre hacia el centro:
