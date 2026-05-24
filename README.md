\# ReservasFodun - Sistema de Reservas



Sistema web de reservas para sedes recreativas y apartamentos, desarrollado como prueba técnica en Microsoft .NET.



El proyecto permite consultar sedes, verificar disponibilidad, calcular tarifas, registrar usuarios, iniciar sesión, recuperar contraseña por correo, crear reservas, consultar reservas realizadas y administrar información base del sistema.



\---



\## 1. Descripción general



\*\*ReservasFodun\*\* es una aplicación web construida con \*\*ASP.NET Core MVC .NET 8\*\*, orientada a la gestión de reservas de sedes recreativas y alojamientos.



La solución fue desarrollada aplicando arquitectura por capas, separación de responsabilidades, Entity Framework Core, SQL Server, Identity, Swagger y procedimientos almacenados.



El sistema cubre el flujo principal solicitado en la prueba técnica:



\- Registro de usuarios.

\- Inicio de sesión.

\- Recuperación de contraseña por correo SMTP.

\- Consulta de sedes recreativas y apartamentos.

\- Consulta de disponibilidad por fechas.

\- Consulta y cálculo de tarifas.

\- Creación de reservas.

\- Consulta de reservas realizadas.

\- Cancelación de reservas.

\- Administración de datos maestros.



\---



\## 2. Tecnologías utilizadas



\- .NET 8

\- ASP.NET Core MVC

\- Razor Pages

\- Entity Framework Core

\- SQL Server 2022

\- SQL Server en Docker

\- ASP.NET Core Identity

\- Swagger / OpenAPI

\- Bootstrap

\- HTML

\- CSS

\- JavaScript

\- smtp4dev

\- Git / GitHub



\---



\## 3. Arquitectura del proyecto



El proyecto está organizado bajo una arquitectura por capas:



```text

ReservasFodun

│

├── src

│   ├── ReservasFodun.Web

│   ├── ReservasFodun.Application

│   ├── ReservasFodun.Domain

│   └── ReservasFodun.Infrastructure

│

├── database

│   ├── 01\_CreateDatabase.sql

│   ├── 02\_CreateTables.sql

│   ├── 03\_SeedData.sql

│   ├── 04\_StoredProcedures.sql

│   ├── 05\_Indexes\_Constraints.sql

│   └── 06\_TestQueries.sql

│

├── docs

├── global.json

└── ReservasFodun.sln

```



\### Capas



\#### ReservasFodun.Web



Capa de presentación. Contiene:



\- Controladores MVC.

\- Controladores API.

\- Vistas Razor.

\- Páginas Identity.

\- Configuración de Swagger.

\- Archivos estáticos.

\- `Program.cs`.



\#### ReservasFodun.Application



Capa de aplicación. Contiene:



\- DTOs.

\- Interfaces de servicios.

\- Interfaces de repositorios.

\- Servicios de aplicación.

\- Reglas de validación del flujo funcional.



\#### ReservasFodun.Domain



Capa de dominio. Contiene:



\- Entidades principales del negocio.

\- Modelos relacionados con sedes, alojamientos, reservas, tarifas, usuarios y servicios.



\#### ReservasFodun.Infrastructure



Capa de infraestructura. Contiene:



\- `ApplicationDbContext`.

\- Configuraciones Fluent API.

\- Repositorios.

\- Conexión con SQL Server.

\- Servicio de correo SMTP.

\- Integración con Identity.



\---



\## 4. Funcionalidades implementadas



\### Usuarios



\- Registro personalizado con ASP.NET Core Identity.

\- Creación de perfil de usuario.

\- Validación de documento duplicado.

\- Inicio de sesión.

\- Cierre de sesión.

\- Recuperación de contraseña por correo SMTP.

\- Edición de perfil.

\- Administración básica de usuarios.



\### Reservas



\- Consulta de sedes.

\- Consulta de disponibilidad por fechas.

\- Consulta de disponibilidad por número de personas.

\- Consulta de tarifas.

\- Cálculo de valor total de reserva.

\- Creación de reserva.

\- Generación de código de reserva.

\- Consulta de mis reservas.

\- Detalle de reserva.

\- Cancelación de reserva.



\### Administración



Módulos administrativos implementados:



\- Administración de sedes.

\- Administración de alojamientos.

\- Administración de tipos de alojamiento.

\- Administración de temporadas.

\- Administración de servicios adicionales.

\- Administración de tarifas.

\- Administración básica de usuarios.



Cada módulo administrativo permite operaciones como:



\- Listar.

\- Ver detalle.

\- Crear.

\- Editar.

\- Activar.

\- Inactivar.



\---



\## 5. Requisitos para ejecutar el proyecto



Antes de ejecutar el proyecto se requiere tener instalado:



\- .NET SDK 8

\- Docker Desktop

\- Git

\- Visual Studio 2022 o Visual Studio Code

\- SQL Server Management Studio o Azure Data Studio, opcional



\---



\## 6. Configuración de SQL Server con Docker



El proyecto utiliza SQL Server 2022 ejecutado en Docker.



\### Crear red Docker



```powershell

docker network create reservas-net

```



\### Crear contenedor SQL Server



```powershell

docker run `

&#x20; -e "ACCEPT\_EULA=Y" `

&#x20; -e "MSSQL\_SA\_PASSWORD=TU\_PASSWORD\_LOCAL" `

&#x20; -p 1433:1433 `

&#x20; --name sqlserver-reservas `

&#x20; --hostname sqlserver-reservas `

&#x20; --network reservas-net `

&#x20; -d mcr.microsoft.com/mssql/server:2022-latest

```



\### Iniciar contenedor si ya existe



```powershell

docker start sqlserver-reservas

```



\### Validar contenedor



```powershell

docker ps

```



\### Validar puerto SQL Server



```powershell

Test-NetConnection localhost -Port 1433

```



Resultado esperado:



```text

TcpTestSucceeded : True

```



\---



\## 7. Configuración de base de datos



Los scripts SQL se encuentran en la carpeta:



```text

database

```



Deben ejecutarse en este orden:



```text

01\_CreateDatabase.sql

02\_CreateTables.sql

03\_SeedData.sql

04\_StoredProcedures.sql

05\_Indexes\_Constraints.sql

06\_TestQueries.sql

```



Base de datos utilizada:



```text

DB\_ReservasFodun

```



\---



\## 8. Cadena de conexión



Configurar la cadena de conexión local en `appsettings.json` o mediante User Secrets.



Ejemplo:



```json

{

&#x20; "ConnectionStrings": {

&#x20;   "DefaultConnection": "Server=tcp:localhost,1433;Database=DB\_ReservasFodun;User Id=sa;Password=TU\_PASSWORD\_LOCAL;TrustServerCertificate=True;"

&#x20; }

}

```



> Nota: Por seguridad, no se recomienda subir contraseñas reales al repositorio.



\---



\## 9. Configuración de smtp4dev



El proyecto usa \*\*smtp4dev\*\* para probar el envío de correos de recuperación de contraseña.



\### Crear contenedor smtp4dev



```powershell

docker run -d `

&#x20; --name smtp4dev-reservas `

&#x20; --network reservas-net `

&#x20; -p 3000:80 `

&#x20; -p 2525:25 `

&#x20; rnwood/smtp4dev

```



\### Iniciar contenedor si ya existe



```powershell

docker start smtp4dev-reservas

```



\### Abrir bandeja de correos



```text

http://localhost:3000

```



\---



\## 10. Ejecutar el proyecto



Ubicarse en la raíz del proyecto:



```powershell

cd "C:\\Ruta\\Del\\Proyecto\\ReservasFodun"

```



Restaurar paquetes:



```powershell

dotnet restore

```



Compilar:



```powershell

dotnet build

```



Ejecutar aplicación web:



```powershell

dotnet run --project "src\\ReservasFodun.Web\\ReservasFodun.Web.csproj"

```



Resultado esperado:



```text

Now listening on: http://localhost:5019

Application started. Press Ctrl+C to shut down.

```



\---



\## 11. URLs principales



\### Aplicación web



```text

http://localhost:5019

```



\### Login



```text

http://localhost:5019/Identity/Account/Login

```



\### Registro



```text

http://localhost:5019/Identity/Account/Register

```



\### Recuperar contraseña



```text

http://localhost:5019/Identity/Account/ForgotPassword

```



\### Sedes



```text

http://localhost:5019/Sedes

```



\### Disponibilidad



```text

http://localhost:5019/Disponibilidad

```



\### Tarifas



```text

http://localhost:5019/Tarifas/Consultar

```



\### Calcular tarifa



```text

http://localhost:5019/Tarifas/Calcular

```



\### Mis reservas



```text

http://localhost:5019/Reservas/MisReservas

```



\### Swagger



```text

http://localhost:5019/swagger

```



\### smtp4dev



```text

http://localhost:3000

```



\---



\## 12. URLs administrativas



```text

http://localhost:5019/AdminSedes

http://localhost:5019/AdminAlojamientos

http://localhost:5019/AdminTiposAlojamiento

http://localhost:5019/AdminTemporadas

http://localhost:5019/AdminServiciosAdicionales

http://localhost:5019/AdminTarifas

http://localhost:5019/AdminUsuarios

```



\---



\## 13. Endpoints principales API / Swagger



El proyecto incluye Swagger para probar los servicios RESTful.



Endpoints principales:



```text

GET /api/sedes

GET /api/sedes/{idSede}



GET /api/disponibilidad



GET /api/tarifas

POST /api/tarifas/calcular



POST /api/reservas

GET /api/reservas/{idReserva}

GET /api/reservas/mis-reservas

PUT /api/reservas/{idReserva}/cancelar

```



\---



\## 14. Pruebas funcionales realizadas



Se realizaron pruebas funcionales completas de:



\- Inicio de aplicación.

\- Conexión con SQL Server Docker.

\- Conexión con smtp4dev.

\- Registro de usuario.

\- Login.

\- Recuperación de contraseña.

\- Consulta de sedes.

\- Consulta de disponibilidad.

\- Consulta de tarifas.

\- Cálculo de tarifa.

\- Creación de reserva.

\- Consulta de mis reservas.

\- Detalle de reserva.

\- Cancelación de reserva.

\- CRUD de sedes.

\- CRUD de alojamientos.

\- CRUD de tipos de alojamiento.

\- CRUD de temporadas.

\- CRUD de servicios adicionales.

\- CRUD de tarifas.

\- Administración de usuarios.

\- Swagger.

\- Validaciones en base de datos.



\---



\## 15. Comando rápido para iniciar ambiente



```powershell

docker start sqlserver-reservas

docker start smtp4dev-reservas

docker ps



dotnet build



dotnet run --project "src\\ReservasFodun.Web\\ReservasFodun.Web.csproj"

```



Abrir:



```text

http://localhost:5019

http://localhost:5019/swagger

http://localhost:3000

```



\---



\## 16. Estado final del proyecto



El proyecto queda funcional para la prueba técnica con los siguientes componentes:



```text

Arquitectura por capas: Completada

Base de datos SQL Server: Completada

Procedimientos almacenados: Completados

Entity Framework Core: Implementado

Identity: Implementado

SMTP recuperación contraseña: Implementado

MVC/Razor: Implementado

Swagger/API: Implementado

Flujo de reservas: Implementado

CRUD administrativo: Implementado

Pruebas funcionales: Completadas

Repositorio GitHub: Publicado

```



\---



\## 17. Consideraciones técnicas



\- El proyecto usa SQL Server en Docker para facilitar la ejecución local.

\- smtp4dev se usa solo como servidor SMTP de pruebas.

\- La administración de usuarios es básica y no implementa roles avanzados.

\- Los módulos administrativos están protegidos por autenticación.

\- La pasarela de pago en línea queda fuera del alcance técnico implementado.

\- El sistema está preparado para futuras mejoras como roles, pagos en línea, auditoría y despliegue en servidor.



\---



\## 18. Autor



\*\*Yeison David Quinto Bolaño\*\*



Prueba técnica: Desarrollador / Analista Desarrollador .NET



Repositorio:



```text

https://github.com/yeisonquintob/P\_T\_Yeison\_Quinto\_ReservasFodun

```

