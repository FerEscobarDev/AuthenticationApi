# AuthenticationApi

Este repositorio implementa una API de autenticación basada en .NET y PostgreSQL. La solución sigue los principios de **Clean Architecture** y emplea **CQRS** para separar operaciones de lectura y escritura.

## Estructura del proyecto

- **AuthenticationApi**: proyecto web ASP.NET Core. Contiene la configuración del host, servicios y controladores de la API.
- **AuthenticationApi.Application**: expone los comandos y consultas de CQRS junto con las interfaces de la capa de aplicación.
- **AuthenticationApi.Domain**: define las entidades del dominio que representan el núcleo de la aplicación.
- **AuthenticationApi.Infrastructure**: implementa repositorios, servicios y el `ApplicationDbContext` usando Entity Framework Core.
- **AuthenticationApi.Shared**: código utilitario y tipos compartidos entre proyectos.

## Compilar la solución

```bash
dotnet build AuthenticationApi.sln
```

## Ejecutar la API

### Con .NET CLI

```bash
dotnet run --project AuthenticationApi
```

La API estará disponible en `https://localhost:8081` en modo desarrollo.

### Con Docker Compose

```bash
docker-compose up --build
```

Este comando levanta la API, una instancia de PostgreSQL y MailPit para pruebas de correo.

## Migraciones de la base de datos

Los scripts incluidos permiten crear y aplicar migraciones de Entity Framework Core.

- `./migrate-dev.sh NombreMigracion` – genera una nueva migración dentro de `AuthenticationApi.Infrastructure`.
- `./update-db.sh` – aplica las migraciones pendientes a la base de datos configurada.

También existen las versiones `.bat` para entornos Windows.

## Clean Architecture y CQRS

La solución se organiza en capas independientes. `Domain` no depende de ningún otro proyecto. `Application` define casos de uso a través de comandos y consultas (CQRS) e interfaces que abstraen la infraestructura. `Infrastructure` implementa dichas interfaces (repositorios, servicios de autenticación, etc.) y `AuthenticationApi` compone todos los servicios y expone los endpoints HTTP.

Los comandos y consultas se encuentran en `AuthenticationApi.Application/Commands` y `AuthenticationApi.Application/Queries`. Cada handler se registra mediante la clase `InfrastructureDependencyInjection` para mantener las dependencias desacopladas.


