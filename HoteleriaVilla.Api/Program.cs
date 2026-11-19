using HoteleriaVilla.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36))));

var app = builder.Build();

// Verificación de conexión a la base de datos al iniciar la aplicación
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        if (dbContext.Database.CanConnect())
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("======================================================================");
            Console.WriteLine("  [OK] CONEXION EXITOSA A LA BASE DE DATOS MYSQL (hoteleria_villa_db) ");
            Console.WriteLine("======================================================================");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("======================================================================");
            Console.WriteLine("  [ERROR] NO SE PUDO CONECTAR A LA BASE DE DATOS MYSQL.                ");
            Console.WriteLine("======================================================================");
            Console.ResetColor();
        }
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("======================================================================");
        Console.WriteLine($"  [ERROR] FALLO DE CONEXION A MYSQL: {ex.Message}");
        Console.WriteLine("======================================================================");
        Console.ResetColor();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
