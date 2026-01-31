UserDirectoryAPI
Overview

UserDirectoryAPI is a .NET 8 Web API that provides CRUD operations for managing user records.
The application is built using Clean Architecture principles and follows SOLID design practices to ensure maintainability, testability, and scalability.

The API exposes RESTful endpoints and uses SQLite as its persistence layer via Entity Framework Core.
OpenAPI/Swagger is enabled for API exploration and testing.

Technology Stack

.NET 8

ASP.NET Core Web API

Entity Framework Core 8

SQLite

Swagger / OpenAPI

Visual Studio 2022

Application Structure (Clean Architecture)
UserDirectoryAPI
│
├── UserDirectoryAPI.Domain
│   └── Entities
│       └── User.cs
│
├── UserDirectoryAPI.Application
│   ├── DTOs
│   │   ├── CreateUserDto.cs
│   │   └── UserDto.cs
│   └── Interfaces
│       └── IUserRepository.cs
│
├── UserDirectoryAPI.Infrastructure
│   ├── Data
│   │   ├── AppDbContext.cs
│   │   └── AppDbContextFactory.cs
│   └── Repositories
│       └── UserRepository.cs
│
└── UserDirectoryAPI.API
    ├── Controllers
    │   └── UsersController.cs
    ├── Program.cs
    └── appsettings.json

Layer Responsibilities

Domain: Core business entities (no dependencies).

Application: DTOs, interfaces, and validation rules.

Infrastructure: Database access, EF Core, repository implementations.

API: HTTP endpoints, dependency injection, Swagger configuration.

Dependencies always point inward, enforcing separation of concerns.

API Endpoints
Method	Endpoint	Description
GET	/api/users	Get all users
GET	/api/users/{id}	Get user by ID
POST	/api/users	Create a new user
PUT	/api/users/{id}	Update an existing user
DELETE	/api/users/{id}	Delete a user
User Model Validation

The API enforces input validation using Data Annotations on DTOs:

Name: required, 2–100 characters

Age: required, integer between 0–120

City: required

State: required

Pincode: required, 4–10 characters

Invalid requests automatically return 400 Bad Request with detailed validation errors.

Database Configuration

Provider: SQLite

Database file: data/app.db (configurable via appsettings.json)

ORM: Entity Framework Core

Migrations: Managed using EF Core migrations with a design-time DbContextFactory

Development Process

Solution created using a multi-project Clean Architecture structure.

Domain and Application layers defined first to establish contracts.

Infrastructure layer implemented using EF Core repositories.

API layer added for HTTP orchestration and dependency injection.

Input validation and correct HTTP status codes enforced.

Swagger enabled for documentation and testing.

EF Core migrations used to manage database schema.

Running the Application

Open the solution in Visual Studio 2022

Set UserDirectoryAPI.API as the startup project

Run database migrations

Press F5

Open Swagger at:

https://localhost:{port}/swagger