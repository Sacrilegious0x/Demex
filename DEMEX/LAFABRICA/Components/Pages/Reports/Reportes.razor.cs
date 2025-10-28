using LAFABRICA.Data.DB;
using LAFABRICA.Models.Interface;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

// Asegúrate que el namespace coincida con tu estructura de proyecto
namespace LAFABRICA.Pages
{
    public partial class ShowReports : ComponentBase, IAsyncDisposable
    {
        
        [Inject] private IOrderService orderService { get; set; }
        [Inject] private IProductService productService { get; set; }
        [Inject] private IClientService clientService { get; set; }
        [Inject] private IClientPaymentService paymentService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private IJSRuntime JSRuntime { get; set; }
        [Inject] private AppDbContext _context { get; set; }

        //  Definiciones de tipos (Pueden ser public o protected) 
        public record SalesDataPoint(string Month, decimal Ventas, int Ordenes);
        public record CategoryDataPoint(string Name, decimal Value, string Color);
        public record TopClientPlaceholder(int Rank, string name, int orders, decimal total);
        public record InventoryAlertPlaceholder(string material, int currentStock, int minStock, string status);

        // --- Propiedades para el estado de la UI ---
        protected bool isLoading = true;
        protected string selectedPeriod = "Últimos 6 meses";
        protected List<string> periodOptions = new() { "Último mes", "Últimos 3 meses", "Últimos 6 meses", "Último año" };
        protected string selectedTab = "sales";

        // --- Propiedades para las métricas  ---
        protected decimal totalSales = 0;
        protected int totalOrders = 0;
        protected decimal avgOrderValue = 0;
        protected int activeClientsCount = 0;

        // --- Listas para los datos ---
        protected List<InventoryAlertPlaceholder> inventoryAlertsPlaceholder = new();
        protected List<TopClientPlaceholder> topClientsPlaceholder = new();
        protected List<KeyValuePair<string, int>> topProductsPlaceholder = new();

        // --- Listas internas  ---
        private List<SalesDataPoint> salesData = new();
        private List<CategoryDataPoint> categoryData = new();

        // --- Ciclos de Vida  ---
        protected override async Task OnInitializedAsync()
        {
            isLoading = true;
            await LoadReportDataAsync();
            isLoading = false;
        }
        
        protected override void OnInitialized()
        {
            
            isLoading = true;
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            // Este método se ejecuta DESPUÉS de que el HTML se renderiza.
            
            if (firstRender)
            {
               
                await LoadReportDataAsync();

                
                isLoading = false;

                
                StateHasChanged();

                
                
                await RenderCharts();
            }
        }

        //  Métodos de carga 
        private async Task LoadReportDataAsync()
        {
            try
            {
                // Definición del Período
                // Establece las fechas de inicio y fin basadas en la selección del dropdown.
                DateTime endDate = DateTime.Now;
                DateTime startDate;
                switch (selectedPeriod)
                {
                    case "Último mes": startDate = endDate.AddMonths(-1); break;
                    case "Últimos 3 meses": startDate = endDate.AddMonths(-3); break;
                    case "Último año": startDate = endDate.AddYears(-1); break;
                    default: startDate = endDate.AddMonths(-6); break;
                }

                DateOnly startDateOnly = DateOnly.FromDateTime(startDate);
                DateOnly endDateOnly = DateOnly.FromDateTime(endDate);

                //  Carga Principal de Datos 
                // Obtiene todas las órdenes activas del período, incluyendo sus clientes
                // y los detalles de producto (ProductOrders) para cálculos posteriores.
                var ordersInPeriod = await _context.Orders
                    .Where(o => o.IsActive == 1 && o.CreationDate >= startDateOnly && o.CreationDate <= endDateOnly)
                    .Include(o => o.Client)
                    .Include(o => o.ProductOrders)
                        .ThenInclude(po => po.IdProductNavigation)
                    .ToListAsync();

                // Cálculo de Métricas (Tarjetas Superiores) 

                // Clientes Activos
                // Cuenta cuántos clientes únicos (agrupando por ID) tienen al menos
                // una orden que NO esté 'Finalizada' en el período.
                activeClientsCount = ordersInPeriod
                    .Where(o => o.State != "Finalizada" && o.Client != null)
                    .GroupBy(o => o.Client.Id)
                    .Count();

                // Ventas Totales
                // Suma el 'TotalAmount' únicamente de las órdenes que están 'Finalizada'.
                totalSales = ordersInPeriod
                    .Where(o => o.State == "Finalizada")
                    .Sum(o => o.TotalAmount);

                // Total Órdenes (Pendientes)
                // Cuenta cuántas órdenes tienen un estado DIFERENTE a 'Finalizada'.
                totalOrders = ordersInPeriod
                    .Where(o => o.State != "Finalizada")
                    .Count();

                // Valor Promedio
                // Divide las Ventas Totales (de finalizadas) entre el número
                // de órdenes finalizadas para obtener el promedio por venta.
                int finalizedOrdersCount = ordersInPeriod.Count(o => o.State == "Finalizada");
                avgOrderValue = finalizedOrdersCount > 0 ? totalSales / finalizedOrdersCount : 0;

                // Preparación de Datos para Gráficos y Pestañas

                // Gráfico de Tendencia de Ventas y Órdenes por Mes
                // Agrupa todas las órdenes por mes y suma sus totales y conteos
                // para mostrar en los gráficos de líneas/barras.
                // NOTA: Este gráfico suma el total de TODAS las órdenes (incluidas pendientes).
                salesData = ordersInPeriod
                    .GroupBy(o => new { o.CreationDate.Year, o.CreationDate.Month })
                    .Select(g => new SalesDataPoint(
                        new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM", new CultureInfo("es-ES")),
                        g.Sum(o => o.TotalAmount),
                        g.Count()
                    ))
                    .OrderBy(d => DateTime.ParseExact(d.Month, "MMM", new CultureInfo("es-ES")))
                    .ToList();

                // Pestaña 'Mejores Clientes'
                // Agrupa las órdenes (sin importar estado) por cliente, suma el total gastado
                // por cada uno, se queda con el Top 5 y les asigna un ranking.
                topClientsPlaceholder = ordersInPeriod
                    .Where(o => o.Client != null)
                    .GroupBy(o => o.Client)
                    .Select(g => new TopClientPlaceholder(
                        Rank: 0, name: g.Key.Name,
                        orders: g.Count(), total: g.Sum(o => o.TotalAmount)
                    ))
                    .OrderByDescending(c => c.total).Take(5)
                    .Select((c, index) => c with { Rank = index + 1 })
                    .ToList();

                // Pestaña 'Productos Más Vendidos'
                // Filtra solo órdenes 'Finalizada', LUEGO expande en líneas de detalle,
                // agrupa por producto, suma cantidades y se queda con el Top 5.
                topProductsPlaceholder = ordersInPeriod
                    .Where(o => o.State == "Finalizada") 
                    .SelectMany(o => o.ProductOrders)
                    .Where(po => po.IdProductNavigation != null)
                    .GroupBy(po => po.IdProductNavigation)
                    .Select(g => new { ProductName = g.Key.Name, TotalQuantity = g.Sum(po => po.Quantity) })
                    .OrderByDescending(p => p.TotalQuantity).Take(5)
                    .Select(p => new KeyValuePair<string, int>(p.ProductName, (int)p.TotalQuantity))
                    .ToList();

                //  Gráfico 'Ventas por Categoría'
                // Filtra solo órdenes 'Finalizada', LUEGO expande,
                // agrupa por 'Categoría' y suma el valor total (Cantidad * Precio).
                categoryData = ordersInPeriod
                    .Where(o => o.State == "Finalizada") 
                    .SelectMany(o => o.ProductOrders)
                    .Where(po => po.IdProductNavigation != null && !string.IsNullOrEmpty(po.IdProductNavigation.Category))
                    .GroupBy(po => po.IdProductNavigation.Category)
                    .Select(g => new CategoryDataPoint(
                        g.Key,
                        g.Sum(po => po.Quantity * po.IdProductNavigation.PriceBase),
                        GetRandomColor()
                    ))
                    .ToList();

                // Pestaña 'Alertas de Inventario' 
                inventoryAlertsPlaceholder = await _context.Inventories
                    // Filtro donde la cantidad actual es menor o igual a la mínima
                    .Where(i => i.Quantity <= i.MinimunQuantity)

                    .Include(i => i.Material)

                    // Proyecta los resultados a tu modelo InventoryAlertPlaceholder 
                    .Select(i => new InventoryAlertPlaceholder(
                        // Posición 1: material
                        i.Material.Name,
                        // Posición 2: currentStock
                        i.Quantity,
                        // Posición 3: minStock
                        i.MinimunQuantity,
                        // Posición 4: status
                        i.Quantity == 0 ? "Agotado" : "Bajo"
                    ))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cargando reportes: {ex.Message}");
            }
        }


        private string GetRandomColor()
        {
            var r = new Random();
            return $"#{r.Next(0x1000000):X6}";
        }

        private object GetChartJsData()
        {
            return new
            {
                SalesLabels = salesData.Select(d => d.Month).ToList(),
                SalesData = salesData.Select(d => d.Ventas).ToList(),
                OrdersData = salesData.Select(d => d.Ordenes).ToList(),
                CategoryLabels = categoryData.Select(c => c.Name).ToList(),
                CategoryData = categoryData.Select(c => c.Value).ToList(),
                CategoryColors = categoryData.Select(c => c.Color).ToList()
            };
        }

        private async Task RenderCharts()
        {
            try
            {
                // Destruye cualquier gráfico existente para evitar conflictos
                await JSRuntime.InvokeVoidAsync("chartInterop.destroyCharts");
                // Renderiza los nuevos  graficos con los datos actuales
                await JSRuntime.InvokeVoidAsync("chartInterop.renderAllCharts", GetChartJsData());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error llamando a JS Interop para gráficos: {ex.Message}");
            }
        }

        //  Manejadores de eventos 
        protected async Task OnPeriodChangedViaEvent(ChangeEventArgs e)
        {
            selectedPeriod = e.Value?.ToString() ?? "Últimos 6 meses";
            isLoading = true;
            StateHasChanged();

            await LoadReportDataAsync();

            isLoading = false;
            StateHasChanged();

            await RenderCharts();
        }


        protected void ExportPlaceholder()
        {
            Console.WriteLine("Exportar presionado");
        }

        protected void GenerateReportPlaceholder()
        {
            Console.WriteLine("Generar Reporte presionado");
        }

        
        protected string FormatPrice(decimal price)
        {
            return price.ToString("C0", new CultureInfo("es-CR"));
        }

        
        public async ValueTask DisposeAsync()
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("chartInterop.destroyCharts");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error llamando a JS Interop para destruir gráficos: {ex.Message}");
            }
        }
    }
}