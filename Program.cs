using Gestion_Quirurgica.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// HTTP CLIENT para consumir otros sistemas
builder.Services.AddHttpClient();

// CORS (IMPORTANTE para conectar frontend)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

// DB Context
builder.Services.AddDbContext<GestionQuirurgicaContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- RAILWAY ---
// Se habilitan Swagger y SwaggerUI fuera del bloque IsDevelopment
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Proyecto_H API V1");
    c.RoutePrefix = string.Empty; // Esto hace que Swagger cargue en la raíz del link
});
// --------------------------------

// 🔴 ACTIVAR CORS (ANTES de MapControllers)
app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();