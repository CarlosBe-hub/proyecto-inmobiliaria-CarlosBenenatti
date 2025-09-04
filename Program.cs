using ProyectoInmobiliaria.Repository;

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

// Registrar RepositorioInmueble 
builder.Services.AddScoped<IInmuebleRepository>(sp =>
    new RepositorioInmueble(builder.Configuration));

// Registrar RepositorioContrato
builder.Services.AddScoped<IContratoRepository>(sp =>
    new ContratoRepository(builder.Configuration));

// 4. Agregar MVC
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
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
