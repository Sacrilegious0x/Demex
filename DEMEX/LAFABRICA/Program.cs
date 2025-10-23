using LAFABRICA.Components;
using LAFABRICA.Data.DB;

using LAFABRICA.Models.Interface;

using LAFABRICA.Services;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
//DATABASE
//Crea variable para la cadena de conexion
var connectionString = builder.Configuration.GetConnectionString("DEMEX");
//registra servicio  para la conexion
builder.Services.AddDbContext<AppDbContext>(options =>
{
options.UseSqlServer(connectionString)
.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information)
           .EnableSensitiveDataLogging();
    });


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBlazorBootstrap();

//Se agregan controladores
builder.Services.AddControllers();

builder.Services.AddScoped<ISupplierService,SupplierService>();


//DEPENDENCIAS 
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IMaterialService, MaterialService>();
builder.Services.AddScoped<IClientPaymentService, ClientPaymentService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddBlazorBootstrap();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

//Que nos mapee los controladores
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
