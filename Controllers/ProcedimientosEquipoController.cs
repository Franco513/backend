using Gestion_Quirurgica.Data;
using Gestion_Quirurgica.Dominio;
using Gestion_Quirurgica.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gestion_Quirurgica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProcedimientosEquipoController : ControllerBase
    {
        private readonly GestionQuirurgicaContext _context;
        public ProcedimientosEquipoController(GestionQuirurgicaContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProcedimientoEquipoResponseDTO>>> GetAll()
        {
            var registros = await (
                from pe   in _context.ProcedimientosEquipo
                where pe.Estado == "Activo"
                join proc in _context.ProcedimientosQuirurgicos on pe.ProcedimientoId equals proc.Id
                join eq   in _context.EquiposMedicos            on pe.EquipoId        equals eq.Id
                select new ProcedimientoEquipoResponseDTO
                {
                    Codigo               = pe.Codigo,
                    CodigoProcedimiento  = proc.Codigo,
                    CodigoEquipo         = eq.Codigo,
                    NombreEquipo         = eq.Nombre,
                    EstadoUso            = pe.EstadoUso,
                    Observaciones        = pe.Observaciones
                }
            ).ToListAsync();

            return Ok(registros);
        }

        [HttpGet("{codigo}")]
        public async Task<ActionResult<ProcedimientoEquipoResponseDTO>> GetByCodigo(string codigo)
        {
            var registro = await (
                from pe   in _context.ProcedimientosEquipo
                where pe.Codigo == codigo && pe.Estado == "Activo"
                join proc in _context.ProcedimientosQuirurgicos on pe.ProcedimientoId equals proc.Id
                join eq   in _context.EquiposMedicos            on pe.EquipoId        equals eq.Id
                select new ProcedimientoEquipoResponseDTO
                {
                    Codigo               = pe.Codigo,
                    CodigoProcedimiento  = proc.Codigo,
                    CodigoEquipo         = eq.Codigo,
                    NombreEquipo         = eq.Nombre,
                    EstadoUso            = pe.EstadoUso,
                    Observaciones        = pe.Observaciones
                }
            ).FirstOrDefaultAsync();

            if (registro is null)
                return NotFound(new { mensaje = $"Registro '{codigo}' no encontrado." });

            return Ok(registro);
        }

        [HttpGet("detalle-completo")]
        public async Task<ActionResult<IEnumerable<MaterialReservadoDTO>>> GetDetalleCompleto()
        {
            var resultado = await (
                from pe   in _context.ProcedimientosEquipo
                where pe.Estado == "Activo"
                join proc in _context.ProcedimientosQuirurgicos on pe.ProcedimientoId equals proc.Id
                where proc.Estado == "Activo"
                join eq   in _context.EquiposMedicos on pe.EquipoId equals eq.Id
                where eq.Estado == "Activo"
                join pac  in _context.Pacientes on proc.PacienteId equals pac.Id
                where pac.Estado == "Activo"
                select new MaterialReservadoDTO
                {
                    CodigoAsignacion    = pe.Codigo,
                    CodigoEquipo        = eq.Codigo,
                    NombreEquipo        = eq.Nombre,
                    Fabricante          = eq.Fabricante,
                    CodigoProcedimiento = proc.Codigo,
                    Paciente            = pac.Nombre,
                    HoraInicio          = proc.HoraInicio,
                    Observaciones       = pe.Observaciones
                }
            ).ToListAsync();

            return Ok(resultado);
        }

        [HttpGet("por-procedimiento/{codigoProcedimiento}")]
        public async Task<ActionResult<IEnumerable<ProcedimientoEquipoResponseDTO>>> GetPorProcedimiento(string codigoProcedimiento)
        {
            var resultado = await (
                from pe   in _context.ProcedimientosEquipo
                where pe.Estado == "Activo"
                join proc in _context.ProcedimientosQuirurgicos on pe.ProcedimientoId equals proc.Id
                where proc.Estado == "Activo" && proc.Codigo == codigoProcedimiento
                join eq   in _context.EquiposMedicos on pe.EquipoId equals eq.Id
                where eq.Estado == "Activo"
                select new ProcedimientoEquipoResponseDTO
                {
                    Codigo               = pe.Codigo,
                    CodigoProcedimiento  = proc.Codigo,
                    CodigoEquipo         = eq.Codigo,
                    NombreEquipo         = eq.Nombre,
                    EstadoUso            = pe.EstadoUso,
                    Observaciones        = pe.Observaciones
                }
            ).ToListAsync();

            return Ok(resultado);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromQuery] ProcedimientoEquipoRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Codigo))
                return BadRequest(new { mensaje = "El campo 'codigo' es obligatorio." });

            if (await _context.ProcedimientosEquipo.AnyAsync(pe => pe.Codigo == dto.Codigo))
                return Conflict(new { mensaje = $"Ya existe un registro con código '{dto.Codigo}'." });

            var proc = await _context.ProcedimientosQuirurgicos
                .FirstOrDefaultAsync(p => p.Codigo == dto.CodigoProcedimiento && p.Estado == "Activo");
            if (proc is null)
                return NotFound(new { mensaje = $"Procedimiento '{dto.CodigoProcedimiento}' no encontrado." });

            var eq = await _context.EquiposMedicos
                .FirstOrDefaultAsync(e => e.Codigo == dto.CodigoEquipo && e.Estado == "Activo");
            if (eq is null)
                return NotFound(new { mensaje = $"Equipo '{dto.CodigoEquipo}' no encontrado." });

            _context.ProcedimientosEquipo.Add(new ProcedimientoEquipo
            {
                Codigo          = dto.Codigo,
                ProcedimientoId = proc.Id,
                EquipoId        = eq.Id,
                EstadoUso       = dto.EstadoUso,
                Observaciones   = dto.Observaciones,
                Estado          = "Activo"
            });

            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetByCodigo), new { codigo = dto.Codigo },
                new { mensaje = "Equipo asignado al procedimiento correctamente.", codigo = dto.Codigo });
        }

        [HttpPut("{codigo}")]
        public async Task<ActionResult> Update(string codigo, [FromQuery] ProcedimientoEquipoRequestDTO dto)
        {
            var pe = await _context.ProcedimientosEquipo
                .FirstOrDefaultAsync(pe => pe.Codigo == codigo && pe.Estado == "Activo");
            if (pe is null)
                return NotFound(new { mensaje = $"Registro '{codigo}' no encontrado." });

            var proc = await _context.ProcedimientosQuirurgicos
                .FirstOrDefaultAsync(p => p.Codigo == dto.CodigoProcedimiento && p.Estado == "Activo");
            if (proc is null)
                return NotFound(new { mensaje = $"Procedimiento '{dto.CodigoProcedimiento}' no encontrado." });

            var eq = await _context.EquiposMedicos
                .FirstOrDefaultAsync(e => e.Codigo == dto.CodigoEquipo && e.Estado == "Activo");
            if (eq is null)
                return NotFound(new { mensaje = $"Equipo '{dto.CodigoEquipo}' no encontrado." });

            pe.ProcedimientoId = proc.Id;
            pe.EquipoId        = eq.Id;
            pe.EstadoUso       = dto.EstadoUso;
            pe.Observaciones   = dto.Observaciones;

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Registro actualizado correctamente." });
        }

        [HttpDelete("{codigo}")]
        public async Task<ActionResult> Delete(string codigo)
        {
            var pe = await _context.ProcedimientosEquipo
                .FirstOrDefaultAsync(pe => pe.Codigo == codigo && pe.Estado == "Activo");
            if (pe is null)
                return NotFound(new { mensaje = $"Registro '{codigo}' no encontrado." });

            pe.Estado = "Inactivo";
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = $"Registro '{codigo}' marcado como Inactivo (Soft Delete)." });
        }
    }
}
