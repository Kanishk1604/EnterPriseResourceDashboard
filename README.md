Enterprise Resource Dashboard (ERD)

A full-stack enterprise resource management system built with ASP.NET Core, Entity Framework Core, SQL Server, and a minimal Angular frontend.
The application supports secure authentication, role-based access control, asset management, and audit logging.


Tech Stack

Backend
	•	ASP.NET Core Web API
	•	Entity Framework Core
	•	SQL Server (Dockerized)
	•	JWT Authentication
	•	Role-Based Access Control (RBAC)
	•	Swagger / OpenAPI

Frontend
	•	Angular (Standalone Components)
	•	HttpClient + Interceptors
	•	JWT-based authentication
	•	Pagination & filtering

Infrastructure
	•	Docker (SQL Server)
	•	RESTful API architecture


Core Features

Authentication & Authorization
	•	User registration and login using JWT
	•	Secure password hashing
	•	Role-based access (Admin, Manager, Employee)
	•	JWT claims include user ID, email, and role
	•	Protected endpoints using [Authorize]


Asset Management
	•	Create, view, assign, and unassign enterprise assets
	•	Assets include:
	•	Name
	•	Asset Tag
	•	Asset Type
	•	Assigned User
	•	Role-based permissions for asset operations
	•	Server-side pagination (Skip / Take)

Audit Logging
	•	Automatic audit logs for critical actions:
	•	Asset creation
	•	Assignment / unassignment
	•	Admin-only endpoint to view audit logs
	•	Stored with timestamps and performing user

Angular Frontend
	•	Login page with JWT authentication
	•	Asset listing with pagination
	•	Role-aware UI behavior
	•	Auth interceptor attaches JWT to API requests
	•	Minimal UI focused on functionality and integration

Running the Project Locally

Prerequisites
	•	.NET SDK
	•	Node.js
	•	Docker

1. Start SQL Server (Docker)
docker run \
  --name erd-sql \
  -e 'ACCEPT_EULA=Y' \
  -e 'MSSQL_SA_PASSWORD=Str0ng!Passw0rd' \
  -e 'MSSQL_PID=Developer' \
  -p 1433:1433 \
  -d mcr.microsoft.com/mssql/server:2022-latest

2. Run Backend
cd backend/Erd.Api
dotnet ef database update
dotnet run

Backend runs at:
http://localhost:5083

Swagger:
http://localhost:5083/swagger


3. Run Frontend
cd erd-web
ng serve

Frontend runs at:
http://localhost:4200

API Highlights
	•	POST /auth/register
	•	POST /auth/login
	•	GET /assets (paginated)
	•	POST /assets
	•	POST /assets/{id}/assign
	•	POST /assets/{id}/unassign
	•	GET /audit-logs (Admin only)


Design Decisions
	•	CSR over SSR: This is an internal enterprise dashboard where SEO is not required. Client-side rendering simplifies authentication and state handling.
	•	Server-side pagination: Prevents loading large datasets into memory.
	•	JWT-based auth: Stateless, scalable, and suitable for APIs.
	•	Minimal Angular UI: Focused on correctness, security, and integration rather than styling.


What This Project Demonstrates
	•	Full-stack system design
	•	Secure authentication and RBAC
	•	Clean separation of concerns
	•	Real-world debugging (JWT, EF Core, Angular hydration)
	•	Enterprise-style backend architecture


Future Improvements
	•	Route guards in Angular
	•	Better UI styling
	•	Search and advanced filtering
	•	Deployment (Azure / AWS)


Author

Kanishk Saini
Software Engineer
