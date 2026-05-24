using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReservasFodun.Application;
using ReservasFodun.Infrastructure;
using ReservasFodun.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// Cadena de conexión SQL Server
// ============================================================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'DefaultConnection'.");

// ============================================================
// DbContext principal del sistema
// IMPORTANTE:
// El ApplicationDbContext correcto está en ReservasFodun.Infrastructure.Data
// No se debe usar ni crear otro ApplicationDbContext en ReservasFodun.Web.
// ============================================================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        connectionString,
        sqlOptions => sqlOptions.MigrationsAssembly("ReservasFodun.Web")
    ));

// ============================================================
// Identity
// ============================================================
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;

    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// ============================================================
// MVC + Razor Pages
// ============================================================
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// ============================================================
// Capas del sistema
// ============================================================
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// ============================================================
// Swagger
// ============================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ============================================================
// Pipeline HTTP
// ============================================================
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ReservasFodun API v1");
        options.RoutePrefix = "swagger";
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// ============================================================
// Middlewares
// ============================================================
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// ============================================================
// Rutas MVC
// ============================================================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ============================================================
// Razor Pages de Identity
// ============================================================
app.MapRazorPages();

app.Run();
