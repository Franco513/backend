using Gestion_Quirurgica.Data;
using Gestion_Quirurgica.Dominio;
using Gestion_Quirurgica.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gestion_Quirurgica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProcedimientosQuirurgicosController : ControllerBase
    {
        private readonly GestionQuirurgicaContext _context;
        public ProcedimientosQuirurgicosController(GestionQuirurgicaContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProcedimientoQuirurgicoResponseDTO>>> GetAll()
        {
            var procedimientos = await (
                from proc in _context.ProcedimientosQuirurgicos
                where proc.Estado == "Activo"
                join pac  in _context.Pacientes        on proc.PacienteId equals pac.Id
                join sala in _context.SalasQuirurgicas on proc.SalaId     equals sala.Id
                select new ProcedimientoQuirurgicoResponseDTO
                {
                    Codigo               = proc.Codigo,
                    CodigoPaciente       = pac.Codigo,
                    NombrePaciente       = pac.Nombre,
                    CodigoSala           = sala.Codigo,
                    NumeroSala           = sala.NumeroSala,
                    HoraInicio           = proc.HoraInicio,
                    HoraFin              = proc.HoraFin,
                    EstadoProcedimiento  = proc.EstadoProcedimiento,
                    Prioridad            = proc.Prioridad
                }
            ).ToListAsync();

            return Ok(procedimientos);
        }

        [HttpGet("{codigo}")]
        public async Task<ActionResult<ProcedimientoQuirurgicoResponseDTO>> GetByCodigo(string codigo)
        {
            var proc = await (
                from p    in _context.ProcedimientosQuirurgicos
                where p.Codigo == codigo && p.Estado == "Activo"
                join pac  in _context.Pacientes        on p.PacienteId equals pac.Id
                join sala in _context.SalasQuirurgicas on p.SalaId     equals sala.Id
                select new ProcedimientoQuirurgicoResponseDTO
                {
                    Codigo               = p.Codigo,
                    CodigoPaciente       = pac.Codigo,
                    NombrePaciente       = pac.Nombre,
                    CodigoSala           = sala.Codigo,
                    NumeroSala           = sala.NumeroSala,
                    HoraInicio           = p.HoraInicio,
                    HoraFin              = p.HoraFin,
                    EstadoProcedimiento  = p.EstadoProcedimiento,
                    Prioridad            = p.Prioridad
                }
            ).FirstOrDefaultAsync();

            if (proc is null)
                return NotFound(new { mensaje = $"Procedimiento '{codigo}' no encontrado." });

            return Ok(proc);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromQuery] ProcedimientoQuirurgicoRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Codigo))
                return BadRequest(new { mensaje = "El campo 'codigo' es obligatorio." });

            if (await _context.ProcedimientosQuirurgicos.AnyAsync(p => p.Codigo == dto.Codigo))
                return Conflict(new { mensaje = $"Ya existe un procedimiento con código '{dto.Codigo}'." });

            var paciente = await _context.Pacientes
                .FirstOrDefaultAsync(p => p.Codigo == dto.CodigoPaciente && p.Estado == "Activo");
            if (paciente is null)
                return NotFound(new { mensaje = $"Paciente '{dto.CodigoPaciente}' no encontrado." });

            var sala = await _context.SalasQuirurgicas
                .FirstOrDefaultAsync(s => s.Codigo == dto.CodigoSala && s.Estado == "Activo");
            if (sala is null)
                return NotFound(new { mensaje = $"Sala '{dto.CodigoSala}' no encontrada." });

            _context.ProcedimientosQuirurgicos.Add(new ProcedimientoQuirurgico
            {
                Codigo              = dto.Codigo,
                PacienteId          = paciente.Id,
                SalaId              = sala.Id,
                HoraInicio          = dto.HoraInicio,
                HoraFin             = dto.HoraFin,
                EstadoProcedimiento = dto.EstadoProcedimiento,
                Prioridad           = dto.Prioridad,
                Estado              = "Activo"
            });

            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetByCodigo), new { codigo = dto.Codigo },
                new { mensaje = "Procedimiento quirúrgico creado correctamente.", codigo = dto.Codigo });
        }

        [HttpPut("{codigo}")]
        public async Task<ActionResult> Update(string codigo, [FromQuery] ProcedimientoQuirurgicoRequestDTO dto)
        {
            var proc = await _context.ProcedimientosQuirurgicos
                .FirstOrDefaultAsync(p => p.Codigo == codigo && p.Estado == "Activo");
            if (proc is null)
                return NotFound(new { mensaje = $"Procedimiento '{codigo}' no encontrado." });

            var paciente = await _context.Pacientes
                .FirstOrDefaultAsync(p => p.Codigo == dto.CodigoPaciente && p.Estado == "Activo");
            if (paciente is null)
                return NotFound(new { mensaje = $"Paciente '{dto.CodigoPaciente}' no encontrado." });

            var sala = await _context.SalasQuirurgicas
                .FirstOrDefaultAsync(s => s.Codigo == dto.CodigoSala && s.Estado == "Activo");
            if (sala is null)
                return NotFound(new { mensaje = $"Sala '{dto.CodigoSala}' no encontrada." });

            proc.PacienteId          = paciente.Id;
            proc.SalaId              = sala.Id;
            proc.HoraInicio          = dto.HoraInicio;
            proc.HoraFin             = dto.HoraFin;
            proc.EstadoProcedimiento = dto.EstadoProcedimiento;
            proc.Prioridad           = dto.Prioridad;

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Procedimiento quirúrgico actualizado correctamente." });
        }

        [HttpDelete("{codigo}")]
        public async Task<ActionResult> Delete(string codigo)
        {
            var proc = await _context.ProcedimientosQuirurgicos
                .FirstOrDefaultAsync(p => p.Codigo == codigo && p.Estado == "Activo");
            if (proc is null)
                return NotFound(new { mensaje = $"Procedimiento '{codigo}' no encontrado." });

            proc.Estado = "Inactivo";
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = $"Procedimiento '{codigo}' marcado como Inactivo (Soft Delete)." });
        }
    }
}
