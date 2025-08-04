# 🛒 DSW2025 - Plataforma E-commerce (Módulo Órdenes y Productos)

Este proyecto corresponde al Trabajo Práctico Integrador de la materia **Desarrollo de Software** (DSW2025), y consiste en la implementación del backend de una plataforma E-commerce en .NET Core, específicamente el módulo de gestión de productos y órdenes.

## 👥 Integrantes

|     Apellido, Nombre      | Legajo |                         Rol                          |
|---------------------------|--------|------------------------------------------------------|
| Gastón, Marcos Villarreal | 48314  | Empezó a programar la solución en otro repositorio   |
| Matías, Daniel Autino     | 55802  | Terminó de programar la solución en este ropositorio |

---

## ⚙️ Tecnologías Utilizadas

- **Lenguaje y Framework:** C# con ASP.NET Core (.NET 8.0)
- **ORM:** Entity Framework Core 9.0.6
- **Base de Datos:** SQL Server
- **Autenticación:** JWT Bearer (Microsoft.AspNetCore.Authentication.JwtBearer 8.0.18)
- **Documentación:** Swagger (Swashbuckle.AspNetCore 6.6.2)

---

## 🗂 Estructura del Proyecto

- `Dsw2025Tpi.Api` - API principal con controladores y configuración
- `Dsw2025Tpi.Application` - Servicios de aplicación y DTOs
- `Dsw2025Tpi.Domain` - Entidades del modelo de dominio
- `Dsw2025Tpi.Data` - Repositorio EF Core, DbContext y carga de datos

---

## 🚀 Cómo ejecutar localmente

### 1. Requisitos previos

- [.NET SDK 8.0+](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads)
- Visual Studio 2022

### 2. Clonar el repositorio

```bash
git clone https://github.com/matiaseas/Dsw2025TpiAutino.git
cd Dsw2025TpiAutino
```

### 3. Configurar la cadena de conexión

En `appsettings.json` dentro del proyecto `Dsw2025Tpi.Api`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=Dsw2025TpiDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  ...
}
```

### 4. Cargar clientes desde archivo JSON

El archivo `customers.json` se encuentra en `Dsw2025Tpi.Data/Seed/customers.json`. Este se utiliza para poblar la tabla de clientes automáticamente al iniciar.

### 5. Aplicar migraciones y correr la API

```bash
dotnet ef database update --project Dsw2025Tpi.Data --startup-project Dsw2025Tpi.Api
dotnet run --project Dsw2025Tpi.Api
```

Swagger estará disponible en:

```
https://localhost:5001/swagger
```

---

## 📦 Endpoints Implementados

### Productos

| Método | Ruta                | Descripción                                  |
|--------|---------------------|----------------------------------------------|
| POST   | /api/products       | Crear nuevo producto                         |
| GET    | /api/products       | Listar productos disponibles                 |
| GET    | /api/products/{id}  | Obtener producto por ID                      |
| PUT    | /api/products/{id}  | Actualizar un producto                       |
| PATCH  | /api/products/{id}  | Inhabilitar un producto (`IsActive = false`) |

### Órdenes

| Método | Ruta                   | Descripción                                  |
|--------|------------------------|----------------------------------------------|
| POST   | /api/orders            | Crear nueva orden                            |
| GET    | /api/orders            | Listar órdenes (filtrado y paginación)       |
| GET    | /api/orders/{id}       | Ver detalles de una orden específica         |
| PUT    | /api/orders/{id}/status| Cambiar el estado de una orden               |

---

## 🔐 Seguridad y Validaciones

- Autenticación JWT en endpoints protegidos (por implementar)
- Validaciones de entrada con `DataAnnotations` y lógica personalizada
- Verificación de stock en creación de órdenes
- Manejo de errores con códigos HTTP claros (400, 404, 500, etc.)

---

## ✅ Estado del Proyecto

✔ Módulo de productos y órdenes  
✔ Carga inicial de clientes desde JSON  
✔ DBContext completo con relaciones y restricciones  
✔ Servicios y controladores operativos  
🔜 Autenticación y autorización JWT (implementado más no testeado)

---

## 🧪 Tests

Los endpoints pueden probarse desde Swagger (`/swagger`) o utilizando Postman/Insomnia.
