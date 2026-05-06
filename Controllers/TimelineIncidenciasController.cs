using Gestion_Quirurgica.Data;
using Gestion_Quirurgica.Dominio;
using Gestion_Quirurgica.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gestion_Quirurgica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimelineIncidenciasController : ControllerBase
    {
        private readonly GestionQuirurgicaContext _context;
        public TimelineIncidenciasController(GestionQuirurgicaContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TimelineIncidenciaResponseDTO>>> GetAll()
        {
            var eventos = await (
                from t    in _context.TimelineIncidencias
                where t.Estado == "Activo"
                join proc in _context.ProcedimientosQuirurgicos on t.ProcedimientoId equals proc.Id
                orderby t.Timestamp ascending
                select new TimelineIncidenciaResponseDTO
                {
                    Codigo               = t.Codigo,
                    CodigoProcedimiento  = proc.Codigo,
                    Timestamp            = t.Timestamp,
                    Tipo                 = t.Tipo,
                    Descripcion          = t.Descripcion,
                    UsuarioRegistra      = t.UsuarioRegistra
                }
            ).ToListAsync();

            return Ok(eventos);
        }

        [HttpGet("{codigo}")]
        public async Task<ActionResult<TimelineIncidenciaResponseDTO>> GetByCodigo(string codigo)
        {
            var evento = await (
                from t    in _context.TimelineIncidencias
                where t.Codigo == codigo && t.Estado == "Activo"
                join proc in _context.ProcedimientosQuirurgicos on t.ProcedimientoId equals proc.Id
                select new TimelineIncidenciaResponseDTO
                {
                    Codigo               = t.Codigo,
                    CodigoProcedimiento  = proc.Codigo,
                    Timestamp            = t.Timestamp,
                    Tipo                 = t.Tipo,
                    Descripcion          = t.Descripcion,
                    UsuarioRegistra      = t.UsuarioRegistra
                }
            ).FirstOrDefaultAsync();

            if (evento is null)
                return NotFound(new { mensaje = $"Evento '{codigo}' no encontrado." });

            return Ok(evento);
        }

        [HttpGet("por-procedimiento/{codigoProcedimiento}")]
        public async Task<ActionResult<IEnumerable<TimelineIncidenciaResponseDTO>>> GetPorProcedimiento(string codigoProcedimiento)
        {
            var timeline = await (
                from t    in _context.TimelineIncidencias
                where t.Estado == "Activo"
                join proc in _context.ProcedimientosQuirurgicos on t.ProcedimientoId equals proc.Id
                where proc.Codigo == codigoProcedimiento && proc.Estado == "Activo"
                orderby t.Timestamp ascending
                select new TimelineIncidenciaResponseDTO
                {
                    Codigo               = t.Codigo,
                    CodigoProcedimiento  = proc.Codigo,
                    Timestamp            = t.Timestamp,
                    Tipo                 = t.Tipo,
                    Descripcion          = t.Descripcion,
                    UsuarioRegistra      = t.UsuarioRegistra
                }
            ).ToListAsync();

            return Ok(timeline);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromQuery] TimelineIncidenciaRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Codigo))
                return BadRequest(new { mensaje = "El campo 'codigo' es obligatorio." });

            if (await _context.TimelineIncidencias.AnyAsync(t => t.Codigo == dto.Codigo))
                return Conflict(new { mensaje = $"Ya existe un evento con código '{dto.Codigo}'." });

            var proc = await _context.ProcedimientosQuirurgicos
                .FirstOrDefaultAsync(p => p.Codigo == dto.CodigoProcedimiento && p.Estado == "Activo");
            if (proc is null)
                return NotFound(new { mensaje = $"Procedimiento '{dto.CodigoProcedimiento}' no encontrado." });

            _context.TimelineIncidencias.Add(new TimelineIncidencia
            {
                Codigo          = dto.Codigo,
                ProcedimientoId = proc.Id,
                Timestamp       = dto.Timestamp,
                Tipo            = dto.Tipo,
                Descripcion     = dto.Descripcion,
                UsuarioRegistra = dto.UsuarioRegistra,
                Estado          = "Activo"
            });

            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetByCodigo), new { codigo = dto.Codigo },
                new { mensaje = "Evento de timeline registrado correctamente.", codigo = dto.Codigo });
        }

        [HttpPut("{codigo}")]
        public async Task<ActionResult> Update(string codigo, [FromQuery] TimelineIncidenciaRequestDTO dto)
        {
            var evento = await _context.TimelineIncidencias
                .FirstOrDefaultAsync(t => t.Codigo == codigo && t.Estado == "Activo");
            if (evento is null)
                return NotFound(new { mensaje = $"Evento '{codigo}' no encontrado." });

            var proc = await _context.ProcedimientosQuirurgicos
                .FirstOrDefaultAsync(p => p.Codigo == dto.CodigoProcedimiento && p.Estado == "Activo");
            if (proc is null)
                return NotFound(new { mensaje = $"Procedimiento '{dto.CodigoProcedimiento}' no encontrado." });

            evento.ProcedimientoId = proc.Id;
            evento.Timestamp       = dto.Timestamp;
            evento.Tipo            = dto.Tipo;
            evento.Descripcion     = dto.Descripcion;
            evento.UsuarioRegistra = dto.UsuarioRegistra;

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Evento de timeline actualizado correctamente." });
        }

        [HttpDelete("{codigo}")]
        public async Task<ActionResult> Delete(string codigo)
        {
            var evento = await _context.TimelineIncidencias
                .FirstOrDefaultAsync(t => t.Codigo == codigo && t.Estado == "Activo");
            if (evento is null)
                return NotFound(new { mensaje = $"Evento '{codigo}' no encontrado." });

            evento.Estado = "Inactivo";
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = $"Evento '{codigo}' marcado como Inactivo (Soft Delete)." });
        }
    }
}
