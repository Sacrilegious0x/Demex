using LAFABRICA.Components;
using LAFABRICA.Data.DB;
using LAFABRICA.Services;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

//DATABASE
//Crea variable para la cadena de conexion
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//registra servicio  para la conexion
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddBlazorBootstrap();

//Se agregan controladores
builder.Services.AddControllers();

builder.Services.AddScoped<ISupplierService,SupplierService>();


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
