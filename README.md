# AuthJwtApi

Simple Authentication API using JWT with **C# .NET 6**.

## Features
- Register new user (with password hashing)
- Login (returns JWT)
- Protected endpoints (require Bearer token)
- Swagger UI for testing

## Tech
- .NET 6
- ASP.NET Core Web API
- JWT (JSON Web Tokens)
- In-memory user store (for demo/dev)

## How to run

1. Clone the repo:
```bash
git clone https://github.com/pacheba/AuthJwtApi.git
cd AuthJwtApi
```
2. Restore and run:
```bash
dotnet restore
dotnet run
```
3. Open Swagger:
```bash
https://localhost:5001/swagger
```
## Endpoints

### POST /api/auth/register
 
Body:

```json
{
  "username": "joao",
  "email": "joao@example.com",
  "password": "P@ssw0rd"
}
```
### POST /api/auth/login

Body:

```json
{
  "username": "joao",
  "password": "P@ssw0rd"
}
```

Returns:
```json
{
  "token": "...",
  "expiresInMinutes": 60
}
```
### GET /api/users (protected)

Header:

```makefile
Authorization: Bearer <token>
```
### GET /api/users/me (protected)

Return logged user based on token.

## Notes

This project uses an in-memory user store for demonstration. For production use, connect to a real DB (EF Core + migrations) and store secrets in secure places (Azure Key Vault / environment vars).

Replace the Jwt:Key in appsettings.json with a secure, random key before publishing.

