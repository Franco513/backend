using Gestion_Quirurgica.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 🔥 CONFIGURAR CONEXIÓN DESDE VARIABLE DE ENTORNO (Render)
var connectionString = Environment.GetEnvironmentVariable("DefaultConnection");

// Si no encuentra la variable, busca en appsettings.json
if (string.IsNullOrEmpty(connectionString))
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
}

// 🔥 CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Gestión Quirúrgica API",
        Version = "v1",
        Description = "Microservicio Gestión Quirúrgica · UPDS · 5FN · MIS · .NET 8 + PostgreSQL"
    });
    options.TagActionsBy(api => new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] });
});

// ✅ Usar la connection string que viene de variable de entorno o appsettings
builder.Services.AddDbContext<GestionQuirurgicaContext>(options =>
    options.UseNpgsql(connectionString)
);

var app = builder.Build();

// 🔥 ACTIVAR CORS
app.UseCors("AllowAll");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GestionQuirurgicaContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Gestión Quirúrgica v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();