# Trabajo Práctico Integrador - Módulo de Órdenes (DSW)

Este proyecto es una API RESTful desarrollada en ASP.NET Core. Implementa el backend del módulo de Órdenes y Productos para una plataforma de E-commerce, gestionando la lógica de negocio, el acceso a datos y la seguridad.

---

## 👥 Integrantes

|     Apellido, Nombres     | Legajo |                               Rol                              |
| ------------------------- |  ----  |----------------------------------------------------------------|
| Gastón, Marcos Villarreal | 48314  | Empezó a programar la solución en otro repositorio             |
| Matías, Daniel Autino     | 55802  | Terminó de programar la solución y la migró a este repositorio |

---

## ⚙️ Tecnologías Utilizadas

-   **Lenguaje y Framework:** C# con ASP.NET Core (.NET 8.0)
-   **Base de Datos:** SQL Server
-   **ORM:** Entity Framework Core
-   **Autenticación:** JWT Bearer
-   **Validación:** FluentValidation
-   **Documentación de API:** Swagger (Swashbuckle)

---

## 🚀 Configuración y Ejecución Local

Debo estos pasos para levantar el proyecto en mi máquina local.

### **1. Prerrequisitos**

-   [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
-   Un motor de base de datos SQL Server (MySQL).

### **2. Clonar el Repositorio**

```bash
git clone [https://github.com/matiaseas/Dsw2025TpiAutino.git](https://github.com/matiaseas/Dsw2025TpiAutino.git)
cd Dsw2025TpiAutino
```

### **3. Configurar la Base de Datos**

1.  Abro el archivo `Dsw2025TpiAutino.Api/appsettings.Development.json`.
2.  Modifico la cadena de conexión (`DefaultConnection`) para que apunte a mi instancia de SQL Server.

    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=TU_SERVIDOR;Database=DswTpiAutinoDb;Trusted_Connection=True;TrustServerCertificate=True;"
    }
    ```

    > **Nota:** Reemplazar `TU_SERVIDOR` por el nombre de mi servidor SQL (por ejemplo, `.` si es una instancia local, o `(localdb)\\mssqllocaldb` para LocalDB).

3.  Abro una terminal en la carpeta raíz del proyecto y ejecuto las migraciones de Entity Framework para crear la base de datos y sus tablas:

    ```bash
    dotnet ef database update --project Dsw2025TpiAutino.Infrastructure
    ```

### **4. Ejecutar la Aplicación**

Ejecuto el siguiente comando desde la raíz del proyecto para iniciar la API:

```bash
dotnet run --project Dsw2025TpiAutino.Api
```

La API estará disponible en las URLs que indique la consola (generalmente `http://localhost:5000` y `https://localhost:5001`).

---

## 📝 Descripción y Uso de Endpoints

Para interactuar con la API, navego a **`/swagger`** (ej. `https://localhost:5001/swagger`). Desde allí podré probar todos los endpoints.

### **🔑 Autenticación**

Los endpoints protegidos requieren un Token JWT. Para obtener uno:

1.  **Creo usuarios:** El sistema no tiene un endpoint de registro público. Debo crear usuarios (`Admin` y `Customer`) directamente en la tabla `Users` de la base de datos. Debo asegurarme de hashear la contraseña.
2.  **Login:** Utilizo el endpoint `POST /api/auth/login` con las credenciales del usuario que creé para obtener un token.
3.  **Autorizar en Swagger:** Hago clic en el botón `Authorize` en la parte superior derecha de Swagger, y en el cuadro de diálogo pega mi token con el formato `Bearer TU_TOKEN`.

### **Endpoints de Productos (`/api/products`)**

| Método | Ruta                      | Descripción                                     | Autorización |
| :----- | :------------------------ | :---------------------------------------------- | :----------- |
| `GET`  | `/`                       | Obtiene la lista completa de productos.         | Pública      |
| `GET`  | `/{id}`                   | Obtiene un producto específico por su GUID.     | Pública      |
| `POST` | `/`                       | Crea un nuevo producto.                         | `Admin`      |
| `PUT`  | `/{id}`                   | Actualiza un producto existente.                | `Admin`      |
| `DELETE`  | `/{id}`                | Elimina un producto (baja física).              | `Admin`      |

### **Endpoints de Órdenes (`/api/orders`)**

| Método | Ruta                      | Descripción                                                                                                                              | Autorización |
| :----- | :------------------------ | :--------------------------------------------------------------------------------------------------------------------------------------- | :----------- |
| `POST` | `/`                       | Crea una nueva orden. Verifica y descuenta el stock.                                                                                     | `Customer`   |
| `GET`  | `/`                       | Obtiene una lista paginada de órdenes. Los `Admin` ven todo; los `Customer` solo ven sus propias órdenes. Acepta filtros (`status`, `customerId`). | `Admin`, `Customer`   |
| `GET`  | `/{id}`                   | Obtiene los detalles de una orden específica.                                                                                            | `Admin`, `Customer`   |
| `PUT`  | `/{id}/status`            | Actualiza el estado de una orden (ej. de `Pending` a `Processing`).                                                                      | `Admin`      |
