# La Fábrica — Sistema de Administración Demex

Sistema de gestión interna para el taller de artesanía **Demex**, desarrollado con .NET 8 y Blazor Server. Centraliza la administración de inventario, clientes, proveedores, órdenes de trabajo, pagos y giras de entrega en una sola aplicación de red local, sin dependencia de internet ni servicios en la nube.

---

## Contexto del negocio

Demex fabrica productos artesanales con maquinaria moderna (impresoras 3D, cortadoras y grabadoras láser) y un equipo especializado en pintura y acabados. Anteriormente su administración se apoyaba en hojas de Excel, pizarras y WhatsApp, lo que generaba una gestión fragmentada y dependiente de la memoria de la dueña. **La Fábrica** reemplaza ese flujo con una plataforma unificada de fácil uso, orientada a mejorar la eficiencia operativa y permitir la expansión gradual del negocio.

---

## Módulos funcionales

| Módulo | Descripción | Prioridad |
|---|---|---|
| **Órdenes** | Crear, consultar, editar y eliminar órdenes de trabajo con seguimiento de estados (Cotizando → Diseño → Corte → Acabados → Entrega) | Alta |
| **Clientes** | CRUD de clientes con marcado automático de cliente frecuente (≥ 3 compras) y búsqueda por zona para planificación de giras | Alta |
| **Inventario** | Gestión de materiales de producción con alertas de stock bajo y asociación a proveedores | Alta |
| **Proveedores** | Registro de proveedores con sugerencia automática de contacto cuando un material está bajo de stock | Media |
| **Productos** | Catálogo de productos con calculadora de precios basada en materiales y complejidad | Media |
| **Pagos** | Registro de pagos de clientes por orden, pagos a empleados por piezas y conteo mensual SINPES | Media |
| **Giras** | Planificación de giras de entrega con lista de clientes por zona, registro de gastos y resumen de rentabilidad | Baja |
| **Reportes** | Reportes gráficos de ventas, productos y empleados, exportables en PDF | Baja |
| **Usuarios y Roles** | Dos roles: Administrador (acceso total) y Trabajador (acceso restringido) | Alta |

---

## Tecnologías

- .NET 8 / Blazor Server
- Entity Framework Core
- SQLite / SQL Server (configurable)
- HTML/CSS (`wwwroot/app.css`)
- xUnit / MSTest para pruebas unitarias y de UI

---

## Requisitos previos

- .NET SDK 8.x
- Visual Studio 2022 con la carga de trabajo **ASP.NET y desarrollo web**
- (Opcional) Docker

---

## Instalación y configuración

```bash
# 1. Clonar el repositorio
git clone https://git.ucr.ac.cr/GABRIEL.MOYACARAVACA/la-fabrica.git
cd la-fabrica

# 2. Restaurar dependencias y compilar
dotnet restore
dotnet build

# 3. Aplicar migraciones de base de datos
dotnet ef database update --project LAFABRICA
```

Ajusta la cadena de conexión y demás parámetros en `appsettings.json` y `appsettings.Development.json` antes de ejecutar.

---

## Ejecutar la aplicación

```bash
# Desde CLI
dotnet run --project LAFABRICA
```

O abre la solución en **Visual Studio 2022** y presiona `F5`.

La aplicación corre localmente y es accesible desde otros equipos de la red mediante URL local (diseñada para 1 servidor + 2 laptops en red interna).

---

## Estructura del proyecto

```
la-fabrica/
├── Components/          # Componentes Blazor, páginas y layouts (App.razor, MainLayout.razor)
├── Data/DB/             # Entidades del dominio y AppDbContext.cs
├── Services/            # Lógica de negocio y acceso a datos
│   └── Auth/            # Autenticación personalizada (DemexAuthService, PasswordValidator)
├── Models/              # Modelos e interfaces
├── wwwroot/             # Recursos estáticos (CSS, JS, imágenes)
├── Program.cs           # Configuración de la app y DI
├── LAFABRICA.Tests/     # Pruebas unitarias
└── LAFABRICA.UI.Test/   # Pruebas de componentes UI
```

---

## Autenticación

La app usa un proveedor de estado personalizado (`DemexAuthenticationStateProvider`) con dos roles:

- **Administrador** — acceso completo a todos los módulos.
- **Trabajador** — acceso restringido a módulos sensibles.

Consulta `Services/Auth/` para detalles de configuración.

---

## Pruebas

```bash
# Pruebas unitarias
dotnet test LAFABRICA.Tests

# Pruebas de componentes UI
dotnet test LAFABRICA.UI.Test
```

---

## Migraciones de base de datos

```bash
# Crear una nueva migración
dotnet ef migrations add NombreMigracion --project LAFABRICA

# Aplicar migraciones pendientes
dotnet ef database update --project LAFABRICA
```

---

## Alcance y limitaciones

- **Despliegue local únicamente** — no requiere internet ni servicios en la nube.
- **Sin procesamiento de pagos** — el sistema registra movimientos contables, no realiza transacciones.
- **Plataforma objetivo: PC** — no contempla versiones móviles ni aplicaciones adicionales.

---

## Contribuir

1. Crea una rama por feature: `git checkout -b feature/nombre-feature`
2. Commits atómicos con mensajes descriptivos.
3. Abre un Pull Request siguiendo las normas de `.editorconfig` y `CONTRIBUTING.md`.

---

## Comandos de referencia rápida

| Comando | Descripción |
|---|---|
| `dotnet restore` | Restaurar paquetes NuGet |
| `dotnet build` | Compilar el proyecto |
| `dotnet run --project LAFABRICA` | Ejecutar la aplicación |
| `dotnet test` | Ejecutar todas las pruebas |
| `dotnet ef migrations add <Nombre>` | Crear migración de EF Core |
| `dotnet ef database update` | Aplicar migraciones pendientes |
