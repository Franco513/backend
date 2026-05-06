using Gestion_Quirurgica.Data;
using Gestion_Quirurgica.Dominio;
using Gestion_Quirurgica.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gestion_Quirurgica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EquiposMedicosController : ControllerBase
    {
        private readonly GestionQuirurgicaContext _context;
        public EquiposMedicosController(GestionQuirurgicaContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EquipoMedicoResponseDTO>>> GetAll()
        {
            var equipos = await (
                from e in _context.EquiposMedicos
                where e.Estado == "Activo"
                select new EquipoMedicoResponseDTO
                {
                    Codigo              = e.Codigo,
                    Nombre              = e.Nombre,
                    Fabricante          = e.Fabricante,
                    UltimoMantenimiento = e.UltimoMantenimiento,
                    EstadoOperativo     = e.EstadoOperativo
                }
            ).ToListAsync();

            return Ok(equipos);
        }

        [HttpGet("{codigo}")]
        public async Task<ActionResult<EquipoMedicoResponseDTO>> GetByCodigo(string codigo)
        {
            var equipo = await (
                from e in _context.EquiposMedicos
                where e.Codigo == codigo && e.Estado == "Activo"
                select new EquipoMedicoResponseDTO
                {
                    Codigo              = e.Codigo,
                    Nombre              = e.Nombre,
                    Fabricante          = e.Fabricante,
                    UltimoMantenimiento = e.UltimoMantenimiento,
                    EstadoOperativo     = e.EstadoOperativo
                }
            ).FirstOrDefaultAsync();

            if (equipo is null)
                return NotFound(new { mensaje = $"Equipo '{codigo}' no encontrado." });

            return Ok(equipo);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromQuery] EquipoMedicoRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Codigo) || string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest(new { mensaje = "Los campos 'codigo' y 'nombre' son obligatorios." });

            if (await _context.EquiposMedicos.AnyAsync(e => e.Codigo == dto.Codigo))
                return Conflict(new { mensaje = $"Ya existe un equipo con código '{dto.Codigo}'." });

            _context.EquiposMedicos.Add(new EquipoMedico
            {
                Codigo              = dto.Codigo,
                Nombre              = dto.Nombre,
                Fabricante          = dto.Fabricante,
                UltimoMantenimiento = dto.UltimoMantenimiento,
                EstadoOperativo     = dto.EstadoOperativo,
                Estado              = "Activo"
            });

            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetByCodigo), new { codigo = dto.Codigo },
                new { mensaje = "Equipo médico creado correctamente.", codigo = dto.Codigo });
        }

        [HttpPut("{codigo}")]
        public async Task<ActionResult> Update(string codigo, [FromQuery] EquipoMedicoRequestDTO dto)
        {
            var equipo = await _context.EquiposMedicos
                .FirstOrDefaultAsync(e => e.Codigo == codigo && e.Estado == "Activo");

            if (equipo is null)
                return NotFound(new { mensaje = $"Equipo '{codigo}' no encontrado." });

            equipo.Nombre              = dto.Nombre;
            equipo.Fabricante          = dto.Fabricante;
            equipo.UltimoMantenimiento = dto.UltimoMantenimiento;
            equipo.EstadoOperativo     = dto.EstadoOperativo;

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Equipo médico actualizado correctamente." });
        }

        [HttpDelete("{codigo}")]
        public async Task<ActionResult> Delete(string codigo)
        {
            var equipo = await _context.EquiposMedicos
                .FirstOrDefaultAsync(e => e.Codigo == codigo && e.Estado == "Activo");

            if (equipo is null)
                return NotFound(new { mensaje = $"Equipo '{codigo}' no encontrado." });

            equipo.Estado = "Inactivo";
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = $"Equipo '{codigo}' marcado como Inactivo (Soft Delete)." });
        }
    }
}
