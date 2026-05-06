using Gestion_Quirurgica.Data;
using Gestion_Quirurgica.Dominio;
using Gestion_Quirurgica.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gestion_Quirurgica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PacientesController : ControllerBase
    {
        private readonly GestionQuirurgicaContext _context;
        public PacientesController(GestionQuirurgicaContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PacienteResponseDTO>>> GetAll()
        {
            var pacientes = await (
                from p in _context.Pacientes
                where p.Estado == "Activo"
                select new PacienteResponseDTO
                {
                    Codigo         = p.Codigo,
                    Nombre         = p.Nombre,
                    Edad           = p.Edad,
                    DiagnosticoPre = p.DiagnosticoPre,
                    Alergias       = p.Alergias,
                    GrupoSanguineo = p.GrupoSanguineo
                }
            ).ToListAsync();

            return Ok(pacientes);
        }

        [HttpGet("{codigo}")]
        public async Task<ActionResult<PacienteResponseDTO>> GetByCodigo(string codigo)
        {
            var paciente = await (
                from p in _context.Pacientes
                where p.Codigo == codigo && p.Estado == "Activo"
                select new PacienteResponseDTO
                {
                    Codigo         = p.Codigo,
                    Nombre         = p.Nombre,
                    Edad           = p.Edad,
                    DiagnosticoPre = p.DiagnosticoPre,
                    Alergias       = p.Alergias,
                    GrupoSanguineo = p.GrupoSanguineo
                }
            ).FirstOrDefaultAsync();

            if (paciente is null)
                return NotFound(new { mensaje = $"Paciente '{codigo}' no encontrado." });

            return Ok(paciente);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromQuery] PacienteRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Codigo) || string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest(new { mensaje = "Los campos 'codigo' y 'nombre' son obligatorios." });

            if (await _context.Pacientes.AnyAsync(p => p.Codigo == dto.Codigo))
                return Conflict(new { mensaje = $"Ya existe un paciente con código '{dto.Codigo}'." });

            _context.Pacientes.Add(new Paciente
            {
                Codigo         = dto.Codigo,
                Nombre         = dto.Nombre,
                Edad           = dto.Edad,
                DiagnosticoPre = dto.DiagnosticoPre,
                Alergias       = dto.Alergias,
                GrupoSanguineo = dto.GrupoSanguineo,
                Estado         = "Activo"
            });

            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetByCodigo), new { codigo = dto.Codigo },
                new { mensaje = "Paciente creado correctamente.", codigo = dto.Codigo });
        }

        [HttpPut("{codigo}")]
        public async Task<ActionResult> Update(string codigo, [FromQuery] PacienteRequestDTO dto)
        {
            var paciente = await _context.Pacientes
                .FirstOrDefaultAsync(p => p.Codigo == codigo && p.Estado == "Activo");

            if (paciente is null)
                return NotFound(new { mensaje = $"Paciente '{codigo}' no encontrado." });

            paciente.Nombre         = dto.Nombre;
            paciente.Edad           = dto.Edad;
            paciente.DiagnosticoPre = dto.DiagnosticoPre;
            paciente.Alergias       = dto.Alergias;
            paciente.GrupoSanguineo = dto.GrupoSanguineo;

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Paciente actualizado correctamente." });
        }

        [HttpDelete("{codigo}")]
        public async Task<ActionResult> Delete(string codigo)
        {
            var paciente = await _context.Pacientes
                .FirstOrDefaultAsync(p => p.Codigo == codigo && p.Estado == "Activo");

            if (paciente is null)
                return NotFound(new { mensaje = $"Paciente '{codigo}' no encontrado." });

            paciente.Estado = "Inactivo";
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = $"Paciente '{codigo}' marcado como Inactivo (Soft Delete)." });
        }
    }
}
