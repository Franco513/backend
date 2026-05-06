using Gestion_Quirurgica.Data;
using Gestion_Quirurgica.Dominio;
using Gestion_Quirurgica.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gestion_Quirurgica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CirujanosController : ControllerBase
    {
        private readonly GestionQuirurgicaContext _context;
        public CirujanosController(GestionQuirurgicaContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CirujanoResponseDTO>>> GetAll()
        {
            var cirujanos = await (
                from c in _context.Cirujanos
                where c.Estado == "Activo"
                select new CirujanoResponseDTO
                {
                    Codigo          = c.Codigo,
                    Nombre          = c.Nombre,
                    Especialidad    = c.Especialidad,
                    AnosExperiencia = c.AnosExperiencia
                }
            ).ToListAsync();

            return Ok(cirujanos);
        }

        [HttpGet("{codigo}")]
        public async Task<ActionResult<CirujanoResponseDTO>> GetByCodigo(string codigo)
        {
            var cirujano = await (
                from c in _context.Cirujanos
                where c.Codigo == codigo && c.Estado == "Activo"
                select new CirujanoResponseDTO
                {
                    Codigo          = c.Codigo,
                    Nombre          = c.Nombre,
                    Especialidad    = c.Especialidad,
                    AnosExperiencia = c.AnosExperiencia
                }
            ).FirstOrDefaultAsync();

            if (cirujano is null)
                return NotFound(new { mensaje = $"Cirujano '{codigo}' no encontrado." });

            return Ok(cirujano);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromQuery] CirujanoRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Codigo) || string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest(new { mensaje = "Los campos 'codigo' y 'nombre' son obligatorios." });

            if (await _context.Cirujanos.AnyAsync(c => c.Codigo == dto.Codigo))
                return Conflict(new { mensaje = $"Ya existe un cirujano con código '{dto.Codigo}'." });

            _context.Cirujanos.Add(new Cirujano
            {
                Codigo          = dto.Codigo,
                Nombre          = dto.Nombre,
                Especialidad    = dto.Especialidad,
                AnosExperiencia = dto.AnosExperiencia,
                Estado          = "Activo"
            });

            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetByCodigo), new { codigo = dto.Codigo },
                new { mensaje = "Cirujano creado correctamente.", codigo = dto.Codigo });
        }

        [HttpPut("{codigo}")]
        public async Task<ActionResult> Update(string codigo, [FromQuery] CirujanoRequestDTO dto)
        {
            var cirujano = await _context.Cirujanos
                .FirstOrDefaultAsync(c => c.Codigo == codigo && c.Estado == "Activo");

            if (cirujano is null)
                return NotFound(new { mensaje = $"Cirujano '{codigo}' no encontrado." });

            cirujano.Nombre          = dto.Nombre;
            cirujano.Especialidad    = dto.Especialidad;
            cirujano.AnosExperiencia = dto.AnosExperiencia;

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Cirujano actualizado correctamente." });
        }

        [HttpDelete("{codigo}")]
        public async Task<ActionResult> Delete(string codigo)
        {
            var cirujano = await _context.Cirujanos
                .FirstOrDefaultAsync(c => c.Codigo == codigo && c.Estado == "Activo");

            if (cirujano is null)
                return NotFound(new { mensaje = $"Cirujano '{codigo}' no encontrado." });

            cirujano.Estado = "Inactivo";
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = $"Cirujano '{codigo}' marcado como Inactivo (Soft Delete)." });
        }
    }
}
