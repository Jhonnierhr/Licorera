using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Licorera.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configurar la conexión a la base de datos
builder.Services.AddDbContext<GestionNegocioContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar sesiones para el carrito de compras
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de expiración de sesión
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = "LicoreraSession";
});

// Configurar la autenticación
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.Name = "LicoreraAuth";
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
        options.SlidingExpiration = true;
    });

// Configurar la autorización
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireRole("Admin"));
    options.AddPolicy("RequireClienteRole", policy =>
        policy.RequireRole("Cliente"));
    options.AddPolicy("RequireVendedorRole", policy =>
        policy.RequireRole("Vendedor"));
    options.AddPolicy("RequireAdminOrVendedorRole", policy =>
        policy.RequireRole("Admin", "Vendedor"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Usar sesiones antes de la autenticación
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Crear roles por defecto si no existen y usuario administrador
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<GestionNegocioContext>();
        context.Database.Migrate();

        // Crear roles por defecto
        if (!context.Roles.Any())
        {
            context.Roles.AddRange(
                new Role { Nombre = "Admin" },
                new Role { Nombre = "Cliente" },
                new Role { Nombre = "Vendedor" }
            );
            context.SaveChanges();
        }

        // Crear usuario administrador por defecto si no existe
        if (!context.Usuarios.Any(u => u.Email == "admin@grandmasliqueurs.com"))
        {
            var adminRole = context.Roles.First(r => r.Nombre == "Admin");
            
            // Usar el mismo método de hash que el AccountController
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes("Admin123!"));
                var passwordHash = Convert.ToBase64String(hashedBytes);
                
                var adminUser = new Usuario
                {
                    Nombre = "Administrador del Sistema",
                    Email = "admin@grandmasliqueurs.com",
                    PasswordHash = passwordHash,
                    RolId = adminRole.RolId,
                    CreatedAt = DateTime.Now
                };
                
                context.Usuarios.Add(adminUser);
                context.SaveChanges();
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al inicializar la base de datos.");
    }
}

app.Run();
