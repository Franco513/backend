using Gestion_Quirurgica.Data;
using Gestion_Quirurgica.Dominio;
using Gestion_Quirurgica.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gestion_Quirurgica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesQuirurgicosController : ControllerBase
    {
        private readonly GestionQuirurgicaContext _context;
        public RolesQuirurgicosController(GestionQuirurgicaContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RolQuirurgicoResponseDTO>>> GetAll()
        {
            var roles = await (
                from r    in _context.RolesQuirurgicos
                where r.Estado == "Activo"
                join proc in _context.ProcedimientosQuirurgicos on r.ProcedimientoId equals proc.Id
                join cir  in _context.Cirujanos                 on r.CirujanoId      equals cir.Id
                select new RolQuirurgicoResponseDTO
                {
                    Codigo               = r.Codigo,
                    CodigoProcedimiento  = proc.Codigo,
                    CodigoCirujano       = cir.Codigo,
                    NombreCirujano       = cir.Nombre,
                    Rol                  = r.Rol
                }
            ).ToListAsync();

            return Ok(roles);
        }

        [HttpGet("{codigo}")]
        public async Task<ActionResult<RolQuirurgicoResponseDTO>> GetByCodigo(string codigo)
        {
            var rol = await (
                from r    in _context.RolesQuirurgicos
                where r.Codigo == codigo && r.Estado == "Activo"
                join proc in _context.ProcedimientosQuirurgicos on r.ProcedimientoId equals proc.Id
                join cir  in _context.Cirujanos                 on r.CirujanoId      equals cir.Id
                select new RolQuirurgicoResponseDTO
                {
                    Codigo               = r.Codigo,
                    CodigoProcedimiento  = proc.Codigo,
                    CodigoCirujano       = cir.Codigo,
                    NombreCirujano       = cir.Nombre,
                    Rol                  = r.Rol
                }
            ).FirstOrDefaultAsync();

            if (rol is null)
                return NotFound(new { mensaje = $"Rol '{codigo}' no encontrado." });

            return Ok(rol);
        }

        [HttpGet("por-procedimiento/{codigoProcedimiento}")]
        public async Task<ActionResult<IEnumerable<RolQuirurgicoResponseDTO>>> GetPorProcedimiento(string codigoProcedimiento)
        {
            var roles = await (
                from r    in _context.RolesQuirurgicos
                where r.Estado == "Activo"
                join proc in _context.ProcedimientosQuirurgicos on r.ProcedimientoId equals proc.Id
                where proc.Codigo == codigoProcedimiento && proc.Estado == "Activo"
                join cir  in _context.Cirujanos on r.CirujanoId equals cir.Id
                where cir.Estado == "Activo"
                select new RolQuirurgicoResponseDTO
                {
                    Codigo               = r.Codigo,
                    CodigoProcedimiento  = proc.Codigo,
                    CodigoCirujano       = cir.Codigo,
                    NombreCirujano       = cir.Nombre,
                    Rol                  = r.Rol
                }
            ).ToListAsync();

            return Ok(roles);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromQuery] RolQuirurgicoRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Codigo))
                return BadRequest(new { mensaje = "El campo 'codigo' es obligatorio." });

            if (await _context.RolesQuirurgicos.AnyAsync(r => r.Codigo == dto.Codigo))
                return Conflict(new { mensaje = $"Ya existe un rol con código '{dto.Codigo}'." });

            var proc = await _context.ProcedimientosQuirurgicos
                .FirstOrDefaultAsync(p => p.Codigo == dto.CodigoProcedimiento && p.Estado == "Activo");
            if (proc is null)
                return NotFound(new { mensaje = $"Procedimiento '{dto.CodigoProcedimiento}' no encontrado." });

            var cir = await _context.Cirujanos
                .FirstOrDefaultAsync(c => c.Codigo == dto.CodigoCirujano && c.Estado == "Activo");
            if (cir is null)
                return NotFound(new { mensaje = $"Cirujano '{dto.CodigoCirujano}' no encontrado." });

            _context.RolesQuirurgicos.Add(new RolQuirurgico
            {
                Codigo          = dto.Codigo,
                ProcedimientoId = proc.Id,
                CirujanoId      = cir.Id,
                Rol             = dto.Rol,
                Estado          = "Activo"
            });

            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetByCodigo), new { codigo = dto.Codigo },
                new { mensaje = "Rol quirúrgico asignado correctamente.", codigo = dto.Codigo });
        }

        [HttpPut("{codigo}")]
        public async Task<ActionResult> Update(string codigo, [FromQuery] RolQuirurgicoRequestDTO dto)
        {
            var rol = await _context.RolesQuirurgicos
                .FirstOrDefaultAsync(r => r.Codigo == codigo && r.Estado == "Activo");
            if (rol is null)
                return NotFound(new { mensaje = $"Rol '{codigo}' no encontrado." });

            var proc = await _context.ProcedimientosQuirurgicos
                .FirstOrDefaultAsync(p => p.Codigo == dto.CodigoProcedimiento && p.Estado == "Activo");
            if (proc is null)
                return NotFound(new { mensaje = $"Procedimiento '{dto.CodigoProcedimiento}' no encontrado." });

            var cir = await _context.Cirujanos
                .FirstOrDefaultAsync(c => c.Codigo == dto.CodigoCirujano && c.Estado == "Activo");
            if (cir is null)
                return NotFound(new { mensaje = $"Cirujano '{dto.CodigoCirujano}' no encontrado." });

            rol.ProcedimientoId = proc.Id;
            rol.CirujanoId      = cir.Id;
            rol.Rol             = dto.Rol;

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Rol quirúrgico actualizado correctamente." });
        }

        [HttpDelete("{codigo}")]
        public async Task<ActionResult> Delete(string codigo)
        {
            var rol = await _context.RolesQuirurgicos
                .FirstOrDefaultAsync(r => r.Codigo == codigo && r.Estado == "Activo");
            if (rol is null)
                return NotFound(new { mensaje = $"Rol '{codigo}' no encontrado." });

            rol.Estado = "Inactivo";
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = $"Rol '{codigo}' marcado como Inactivo (Soft Delete)." });
        }
    }
}
