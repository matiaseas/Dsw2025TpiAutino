# Explicación Completa del Proyecto E-Commerce DSW2025TpiAutino

## Visión General de la Arquitectura

Este proyecto implementa un backend de E-commerce siguiendo una **arquitectura de capas (Layers)** con principios **RESTful**. La decisión de usar esta arquitectura responde a los requisitos específicos de **seguridad, usabilidad y mantenibilidad** establecidos en el documento de especificaciones.

### ¿Por qué una Arquitectura de Capas?

La arquitectura de capas separa las responsabilidades del sistema en diferentes niveles, lo que permite:
- **Mantenibilidad**: Cada capa tiene una responsabilidad específica
- **Testabilidad**: Puedes probar cada capa independientemente
- **Escalabilidad**: Facilita agregar nuevas funcionalidades sin afectar otras capas
- **Reutilización**: Las capas inferiores pueden ser reutilizadas por diferentes capas superiores

## Estructura del Proyecto (4 Capas Principales)

El proyecto está dividido en 4 proyectos que representan las capas de la arquitectura:

### 1. **Dsw2025Tpi.Api** (Capa de Presentación)
**Responsabilidad**: Exponer endpoints HTTP y manejar requests/responses

```
Dsw2025Tpi.Api/
├── Controllers/           # Controladores REST
│   ├── ProductsController.cs
│   └── OrdersController.cs
├── Program.cs            # Configuración de la aplicación
├── appsettings.json      # Configuraciones (conexión DB, JWT, etc.)
└── Dsw2025Tpi.Api.csproj # Dependencias del proyecto
```

### 2. **Dsw2025Tpi.Application** (Capa de Aplicación)
**Responsabilidad**: Lógica de negocio y orquestación de operaciones

```
Dsw2025Tpi.Application/
├── Services/             # Servicios de aplicación
│   ├── ProductService.cs
│   └── OrderService.cs
├── DTOs/                 # Data Transfer Objects
│   ├── ProductDto.cs
│   ├── OrderDto.cs
│   └── CreateOrderDto.cs
└── Interfaces/           # Contratos de servicios
    ├── IProductService.cs
    └── IOrderService.cs
```

### 3. **Dsw2025Tpi.Domain** (Capa de Dominio)
**Responsabilidad**: Entidades del negocio y reglas de dominio

```
Dsw2025Tpi.Domain/
├── Entities/             # Entidades del modelo de dominio
│   ├── Product.cs
│   ├── Order.cs
│   ├── OrderItem.cs
│   └── Customer.cs
└── Enums/               # Enumeraciones del dominio
    └── OrderStatus.cs
```

### 4. **Dsw2025Tpi.Data** (Capa de Datos)
**Responsabilidad**: Persistencia y acceso a datos

```
Dsw2025Tpi.Data/
├── Context/              # DbContext de Entity Framework
│   └── ApplicationDbContext.cs
├── Repositories/         # Implementación del patrón Repository
│   ├── ProductRepository.cs
│   └── OrderRepository.cs
├── Seed/                 # Datos iniciales
│   └── customers.json
└── Migrations/          # Migraciones de base de datos
```

## Análisis Archivo por Archivo

### Capa de Dominio (Domain) - El Corazón del Negocio

#### **Entities/Product.cs**
```csharp
public class Product
{
    public Guid Id { get; set; }           // GUID como especifica el documento
    public string SKU { get; set; }        // Obligatorio y único
    public string InternalCode { get; set; }
    public string Name { get; set; }       // Obligatorio
    public string Description { get; set; }
    public decimal CurrentUnitPrice { get; set; } // Debe ser > 0
    public int StockQuantity { get; set; } // No puede ser negativo
    public bool IsActive { get; set; }     // Para soft delete
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

**Decisiones de Diseño Importantes**:
- **GUID como ID**: Garantiza unicidad global, especialmente útil en sistemas distribuidos
- **IsActive**: Implementa "soft delete" en lugar de borrar físicamente los productos
- **CurrentUnitPrice**: Separado del precio histórico en OrderItem para mantener integridad

#### **Entities/Order.cs**
```csharp
public class Order
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string ShippingAddress { get; set; }
    public string BillingAddress { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }    // Calculado por el backend
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Relaciones
    public Customer Customer { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; }
}
```

**Aspectos Clave**:
- **TotalAmount calculado**: El backend calcula el total, no confía en el frontend
- **Relaciones navegacionales**: Entity Framework puede cargar datos relacionados automáticamente

#### **Entities/OrderItem.cs**
```csharp
public class OrderItem
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }    // Precio en el momento de la compra
    public decimal Subtotal { get; set; }     // Quantity * UnitPrice
    
    // Relaciones
    public Order Order { get; set; }
    public Product Product { get; set; }
}
```

**¿Por qué UnitPrice en OrderItem?**
Esta es una decisión crucial: guardamos el precio del producto **en el momento exacto de la compra**. Esto es fundamental porque:
- Los precios de productos pueden cambiar con el tiempo
- Las órdenes históricas deben mantener sus precios originales
- Auditoría y trazabilidad de transacciones

### Capa de Datos (Data) - Persistencia

#### **Context/ApplicationDbContext.cs**
```csharp
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Customer> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuraciones de entidades
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.SKU).IsRequired().HasMaxLength(50);
            entity.HasIndex(p => p.SKU).IsUnique(); // SKU único
            entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
            entity.Property(p => p.CurrentUnitPrice).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");
            entity.HasOne(o => o.Customer)
                  .WithMany()
                  .HasForeignKey(o => o.CustomerId);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(oi => oi.Id);
            entity.Property(oi => oi.UnitPrice).HasColumnType("decimal(18,2)");
            entity.Property(oi => oi.Subtotal).HasColumnType("decimal(18,2)");
            entity.HasOne(oi => oi.Order)
                  .WithMany(o => o.OrderItems)
                  .HasForeignKey(oi => oi.OrderId);
            entity.HasOne(oi => oi.Product)
                  .WithMany()
                  .HasForeignKey(oi => oi.ProductId);
        });
    }
}
```

**Configuraciones Importantes**:
- **Índice único en SKU**: Garantiza que no haya SKUs duplicados a nivel de base de datos
- **Tipos decimal(18,2)**: Precisión monetaria adecuada para precios
- **Relaciones explícitas**: Define claramente las foreign keys y navegación

### Capa de Aplicación (Application) - Lógica de Negocio

#### **Services/ProductService.cs**
```csharp
public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
    {
        // Validaciones de negocio
        if (string.IsNullOrWhiteSpace(createProductDto.SKU))
            throw new ValidationException("SKU es obligatorio");

        if (createProductDto.CurrentUnitPrice <= 0)
            throw new ValidationException("El precio debe ser mayor a 0");

        if (createProductDto.StockQuantity < 0)
            throw new ValidationException("El stock no puede ser negativo");

        // Verificar SKU único
        var existingProduct = await _productRepository.GetBySKUAsync(createProductDto.SKU);
        if (existingProduct != null)
            throw new ValidationException("El SKU ya existe");

        // Crear entidad
        var product = new Product
        {
            Id = Guid.NewGuid(),
            SKU = createProductDto.SKU,
            InternalCode = createProductDto.InternalCode,
            Name = createProductDto.Name,
            Description = createProductDto.Description,
            CurrentUnitPrice = createProductDto.CurrentUnitPrice,
            StockQuantity = createProductDto.StockQuantity,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _productRepository.AddAsync(product);
        await _productRepository.SaveChangesAsync();

        return MapToDto(product);
    }

    public async Task<ProductDto> UpdateProductAsync(Guid id, UpdateProductDto updateProductDto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            throw new NotFoundException("Producto no encontrado");

        // Validaciones similares...
        
        // Actualizar propiedades
        product.Name = updateProductDto.Name;
        product.Description = updateProductDto.Description;
        product.CurrentUnitPrice = updateProductDto.CurrentUnitPrice;
        product.StockQuantity = updateProductDto.StockQuantity;
        product.UpdatedAt = DateTime.UtcNow;

        await _productRepository.SaveChangesAsync();
        return MapToDto(product);
    }
}
```

**Patrones Implementados**:
- **Dependency Injection**: Recibe dependencies por constructor
- **DTO Pattern**: Usa DTOs para transfer de datos entre capas
- **Validation**: Valida reglas de negocio antes de persistir
- **Exception Handling**: Lanza excepciones específicas para diferentes errores

#### **Services/OrderService.cs** - La Lógica Más Compleja
```csharp
public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;

    public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto)
    {
        // 1. Validar que el customer existe
        var customer = await _customerRepository.GetByIdAsync(createOrderDto.CustomerId);
        if (customer == null)
            throw new ValidationException("Cliente no encontrado");

        // 2. Verificar stock ANTES de crear la orden
        var productStockValidation = new List<(Guid ProductId, int RequestedQuantity, int AvailableStock)>();
        
        foreach (var orderItem in createOrderDto.OrderItems)
        {
            var product = await _productRepository.GetByIdAsync(orderItem.ProductId);
            if (product == null)
                throw new ValidationException($"Producto {orderItem.ProductId} no encontrado");

            if (!product.IsActive)
                throw new ValidationException($"Producto {product.Name} no está disponible");

            if (product.StockQuantity < orderItem.Quantity)
            {
                throw new ValidationException(
                    $"Stock insuficiente para {product.Name}. " +
                    $"Solicitado: {orderItem.Quantity}, Disponible: {product.StockQuantity}");
            }

            productStockValidation.Add((orderItem.ProductId, orderItem.Quantity, product.StockQuantity));
        }

        // 3. Crear la orden (si llegamos aquí, todo está validado)
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = createOrderDto.CustomerId,
            ShippingAddress = createOrderDto.ShippingAddress,
            BillingAddress = createOrderDto.BillingAddress,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            OrderItems = new List<OrderItem>()
        };

        decimal totalAmount = 0;

        // 4. Crear OrderItems y actualizar stock
        foreach (var orderItemDto in createOrderDto.OrderItems)
        {
            var product = await _productRepository.GetByIdAsync(orderItemDto.ProductId);
            
            var orderItem = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = orderItemDto.ProductId,
                Quantity = orderItemDto.Quantity,
                UnitPrice = product.CurrentUnitPrice, // Precio actual del producto
                Subtotal = orderItemDto.Quantity * product.CurrentUnitPrice
            };

            order.OrderItems.Add(orderItem);
            totalAmount += orderItem.Subtotal;

            // Actualizar stock
            product.StockQuantity -= orderItemDto.Quantity;
            product.UpdatedAt = DateTime.UtcNow;
        }

        order.TotalAmount = totalAmount;

        // 5. Persistir todo en una transacción
        await _orderRepository.AddAsync(order);
        await _orderRepository.SaveChangesAsync();

        return MapToDto(order);
    }
}
```

**Lógica Crítica de Gestión de Stock**:
1. **Verificación previa**: Valida todo antes de modificar cualquier dato
2. **Transaccionalidad**: Si algo falla, nada se persiste
3. **Atomicidad**: O se crea la orden completa o no se crea nada
4. **Integridad**: Mantiene la consistencia entre órdenes y stock

### Capa de Presentación (API) - Endpoints REST

#### **Controllers/ProductsController.cs**
```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto createProductDto)
    {
        try
        {
            var result = await _productService.CreateProductAsync(createProductDto);
            return CreatedAtAction(nameof(GetProduct), new { id = result.Id }, result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
    {
        var products = await _productService.GetAllProductsAsync();
        
        if (!products.Any())
            return NoContent(); // 204 como especifica el documento

        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(Guid id)
    {
        try
        {
            var product = await _productService.GetProductByIdAsync(id);
            return Ok(product);
        }
        catch (NotFoundException)
        {
            return NotFound(new { message = "Producto no encontrado" });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProductDto>> UpdateProduct(Guid id, [FromBody] UpdateProductDto updateProductDto)
    {
        try
        {
            var result = await _productService.UpdateProductAsync(id, updateProductDto);
            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound(new { message = "Producto no encontrado" });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult> DisableProduct(Guid id)
    {
        try
        {
            await _productService.DisableProductAsync(id);
            return NoContent(); // 204 como especifica el documento
        }
        catch (NotFoundException)
        {
            return NotFound(new { message = "Producto no encontrado" });
        }
    }
}
```

**Aspectos Clave del Controller**:
- **Códigos HTTP correctos**: 201 Created, 200 OK, 204 No Content, 400 Bad Request, 404 Not Found, 500 Internal Server Error
- **Exception handling**: Convierte excepciones de negocio en respuestas HTTP apropiadas
- **DTOs**: No expone entidades directamente, usa DTOs para control de datos

#### **Program.cs** - Configuración de la Aplicación
```csharp
var builder = WebApplication.CreateBuilder(args);

// Configurar servicios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// JWT Authentication (preparado para futuro)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// CORS para frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000") // React app típica
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configurar pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigin");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Inicializar base de datos con datos de prueba
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.EnsureCreatedAsync();
    await SeedData.InitializeAsync(context); // Carga customers.json
}

app.Run();
```

## Características de Seguridad Implementadas

### 1. **Validación de Datos**
- **DTOs con DataAnnotations**: Validación en múltiples capas
- **Validación de negocio**: Reglas específicas del dominio
- **SQL Injection prevention**: Entity Framework previene automáticamente

### 2. **Manejo de Errores**
- **Exception handling global**: Middleware para capturar errores no manejados
- **Códigos HTTP específicos**: Respuestas consistentes según el tipo de error
- **No exposición de información sensible**: Los errores internos no revelan detalles del sistema

### 3. **CORS Configurado**
- **Orígenes específicos**: Solo permite requests desde dominios autorizados
- **Headers controlados**: Restringe qué headers pueden enviarse

### 4. **JWT Ready**
- **Autenticación preparada**: Infraestructura lista para implementar autenticación
- **Autorización por roles**: Preparado para diferentes niveles de acceso

## Flujo de Creación de Orden (Caso de Uso Crítico)

Este es el flujo más complejo y crítico del sistema:

1. **Request llega al Controller** → Recibe CreateOrderDto
2. **Controller llama al Service** → Delega lógica de negocio
3. **Service valida Customer** → Verifica que existe
4. **Service verifica stock** → Para TODOS los productos antes de continuar
5. **Service crea Order entity** → Si todo es válido
6. **Service crea OrderItems** → Con precios actuales de productos
7. **Service actualiza stock** → Reduce quantities en productos
8. **Service calcula totales** → Backend calcula, no confía en frontend
9. **Repository persiste** → Guarda en base de datos
10. **Service retorna DTO** → Respuesta estructurada al controller
11. **Controller retorna HTTP 201** → Con la orden creada

## ¿Por qué esta Arquitectura Funciona?

### **Separación de Responsabilidades**
- **Controllers**: Solo manejan HTTP, no lógica de negocio
- **Services**: Contienen toda la lógica de negocio
- **Repositories**: Solo acceso a datos
- **Entities**: Solo modelo de dominio

### **Testabilidad**
- **Interfaces**: Permiten mocking para unit tests
- **Dependency Injection**: Facilita intercambiar implementaciones
- **Capas independientes**: Puedes testear cada capa por separado

### **Escalabilidad**
- **Nuevos endpoints**: Solo agregar controllers y services
- **Nuevas entidades**: Solo extender el modelo de dominio
- **Nuevas validaciones**: Solo modificar services
- **Cambios de persistencia**: Solo modificar repositories

### **Mantenibilidad**
- **Código organizado**: Fácil encontrar donde está cada funcionalidad
- **Responsabilidades claras**: Cada clase tiene un propósito específico
- **Acoplamiento bajo**: Cambios en una capa no afectan las otras

Esta arquitectura cumple perfectamente con todos los requisitos del documento y establece una base sólida para el crecimiento futuro del sistema de E-commerce.