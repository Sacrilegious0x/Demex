using LAFABRICA.Components;
using LAFABRICA.Data.DB;
using LAFABRICA.Models.Interface;
using LAFABRICA.Services;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

//DATABASE
//Crea variable para la cadena de conexion
var connectionString = builder.Configuration.GetConnectionString("DEMEX");
//registra servicio  para la conexion
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
//DEPENDENCIAS 
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddBlazorBootstrap();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
