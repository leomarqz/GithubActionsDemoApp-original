using GithubActionsDemoApp;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(opciones => opciones.UseSqlServer("name=DefaultConnection"));

var app = builder.Build();

// ==========================================
// BLOQUE DE MIGRACIONES AUTOMÁTICAS
// ==========================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        // Esto busca migraciones pendientes y las aplica en Azure SQL
        context.Database.Migrate(); 
    }
    catch (Exception ex)
    {
        // Es vital capturar errores aquí para que la app no muera en silencio
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error al migrar la base de datos en el despliegue.");
    }
}
// ==========================================

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
