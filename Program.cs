using ProyectoInmobiliaria.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using BCrypt.Net; 
using ProyectoInmobiliaria.Models;


var builder = WebApplication.CreateBuilder(args);

// 1. Leer la cadena de conexión desde appsettings.json
var connectionString = builder.Configuration.GetConnectionString("Inmobiliaria");

// 2. Verificar que la cadena de conexión no sea nula o vacía
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("La cadena de conexión 'Inmobiliaria' no está configurada.");
}

// 3. Registrar tus Repositories en el contenedor de dependencias
builder.Services.AddScoped<IPropietarioRepository>(sp =>
    new PropietarioRepository(connectionString));

builder.Services.AddScoped<IInquilinoRepository>(sp =>
    new InquilinoRepository(connectionString));

builder.Services.AddScoped<IInmuebleRepository>(sp =>
    new RepositorioInmueble(builder.Configuration));

builder.Services.AddScoped<IContratoRepository>(sp =>
    new ContratoRepository(builder.Configuration));

// Registrar RepositorioPago
builder.Services.AddScoped<IPagoRepository>(sp =>
    new RepositorioPago(builder.Configuration));

// Registrar RepositorioUsuario
builder.Services.AddScoped<IRepositorioUsuario>(sp =>
    new RepositorioUsuario(connectionString)); // Registrar el repositorio de usuarios

// 4. Agregar soporte para la autenticación y autorización
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Usuarios/Login"; // Cambiado a /Usuarios/Login
        options.LogoutPath = "/Usuarios/Logout"; // Ruta de logout
        options.SlidingExpiration = true; 
    });

// Agregar servicios de MVC
builder.Services.AddControllersWithViews();


var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/500"); // Captura errores 500
    app.UseHsts();
}

// Captura códigos de estado como 404, 403, etc. y redirige a ErrorController
app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication(); 
app.UseAuthorization();

app.UseStaticFiles(); // Para servir recursos estáticos

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Usuarios}/{action=Login}/{id?}")
    .WithStaticAssets();

app.Run();

