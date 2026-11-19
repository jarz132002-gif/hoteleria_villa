# Hotelería Villa - Backend API (.NET 8 + DDD + EF Core + MySQL)

Solución desarrollada bajo arquitectura orientada a **Domain-Driven Design (DDD)** con entidades de dominio ricas, objetos de valor, enums y configuración mediante **Fluent API**, configurada para **MySQL** (`Pomelo.EntityFrameworkCore.MySql`).

## Estructura de la Solución
- **HoteleriaVilla.Domain**: Entidades ricas (12 tablas), Enums y Value Objects.
- **HoteleriaVilla.Infrastructure**: `ApplicationDbContext` y configuraciones Fluent API (`IEntityTypeConfiguration`).
- **HoteleriaVilla.Api**: Controladores REST con los 11 endpoints solicitados, DTOs y Swagger.
- **hoteleria_villa_db.sql**: Script completo de creación de la base de datos y las 12 tablas en MySQL.

## Pasos para ejecutar en Visual Studio
1. Ejecutar el archivo `hoteleria_villa_db.sql` en tu gestor de MySQL para crear `hoteleria_villa_db`.
2. Abrir `HoteleriaVilla.sln` en Visual Studio.
3. Verificar en el Explorador de soluciones que **`HoteleriaVilla.Api`** esté en negrita como proyecto de inicio.
4. Presionar **`F5`** (o botón verde Play): la consola confirmará `[OK] CONEXION EXITOSA A LA BASE DE DATOS MYSQL` y abrirá Swagger (`/swagger`).
