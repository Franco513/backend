using Gestion_Quirurgica.Data;
using Gestion_Quirurgica.Dominio;
using Gestion_Quirurgica.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gestion_Quirurgica.Controllers
{
    /// <summary>
    /// Controlador de Programación de Cirugías.
    ///
    /// PROPÓSITO: Permite que los médicos soliciten la programación de una cirugía
    /// ANTES de que sea aprobada y convertida en un ProcedimientoQuirurgico formal.
    ///
    /// FLUJO:
    ///   1. Médico hace POST /api/ProgramacionCirugia  → Estado: "Solicitada"
    ///   2. Jefatura quirúrgica aprueba o rechaza via PUT .../estado  → "Aprobada" | "Rechazada"
    ///   3. Si aprobada, se puede convertir en ProcedimientoQuirurgico via POST .../ejecutar
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProgramacionCirugiaController : ControllerBase
    {
        private readonly GestionQuirurgicaContext _context;
        public ProgramacionCirugiaController(GestionQuirurgicaContext context) => _context = context;

        // ─────────────────────────────────────────────────────────────────────
        // GET /api/ProgramacionCirugia
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProgramacionCirugiaResponseDTO>>> GetAll()
        {
            var resultado = await (
                from prog in _context.ProgramacionesCirugia
                where prog.Estado == "Activo"
                join pac  in _context.Pacientes        on prog.PacienteId              equals pac.Id
                join sala in _context.SalasQuirurgicas on prog.SalaId                  equals sala.Id
                join cir  in _context.Cirujanos        on prog.CirujanoSolicitanteId   equals cir.Id
                orderby prog.HoraInicioEstimada ascending
                select new ProgramacionCirugiaResponseDTO
                {
                    Codigo               = prog.Codigo,
                    CodigoPaciente       = pac.Codigo,
                    NombrePaciente       = pac.Nombre,
                    CodigoSala           = sala.Codigo,
                    NumeroSala           = sala.NumeroSala,
                    CodigoCirujano       = cir.Codigo,
                    NombreCirujano       = cir.Nombre,
                    FechaSolicitada      = prog.FechaSolicitada,
                    HoraInicioEstimada   = prog.HoraInicioEstimada,
                    HoraFinEstimada      = prog.HoraFinEstimada,
                    TipoCirugia          = prog.TipoCirugia,
                    Prioridad            = prog.Prioridad,
                    Observaciones        = prog.Observaciones,
                    EstadoProgramacion   = prog.EstadoProgramacion
                }
            ).ToListAsync();

            return Ok(resultado);
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET /api/ProgramacionCirugia/{codigo}
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet("{codigo}")]
        public async Task<ActionResult<ProgramacionCirugiaResponseDTO>> GetByCodigo(string codigo)
        {
            var resultado = await (
                from prog in _context.ProgramacionesCirugia
                where prog.Codigo == codigo && prog.Estado == "Activo"
                join pac  in _context.Pacientes        on prog.PacienteId            equals pac.Id
                join sala in _context.SalasQuirurgicas on prog.SalaId                equals sala.Id
                join cir  in _context.Cirujanos        on prog.CirujanoSolicitanteId equals cir.Id
                select new ProgramacionCirugiaResponseDTO
                {
                    Codigo               = prog.Codigo,
                    CodigoPaciente       = pac.Codigo,
                    NombrePaciente       = pac.Nombre,
                    CodigoSala           = sala.Codigo,
                    NumeroSala           = sala.NumeroSala,
                    CodigoCirujano       = cir.Codigo,
                    NombreCirujano       = cir.Nombre,
                    FechaSolicitada      = prog.FechaSolicitada,
                    HoraInicioEstimada   = prog.HoraInicioEstimada,
                    HoraFinEstimada      = prog.HoraFinEstimada,
                    TipoCirugia          = prog.TipoCirugia,
                    Prioridad            = prog.Prioridad,
                    Observaciones        = prog.Observaciones,
                    EstadoProgramacion   = prog.EstadoProgramacion
                }
            ).FirstOrDefaultAsync();

            if (resultado is null)
                return NotFound(new { mensaje = $"Programación '{codigo}' no encontrada." });

            return Ok(resultado);
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET /api/ProgramacionCirugia/pendientes
        // Programaciones solicitadas que están esperando aprobación.
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet("pendientes")]
        public async Task<ActionResult> GetPendientes()
        {
            var resultado = await (
                from prog in _context.ProgramacionesCirugia
                where prog.Estado == "Activo" && prog.EstadoProgramacion == "Solicitada"
                join pac  in _context.Pacientes        on prog.PacienteId            equals pac.Id
                join sala in _context.SalasQuirurgicas on prog.SalaId                equals sala.Id
                join cir  in _context.Cirujanos        on prog.CirujanoSolicitanteId equals cir.Id
                orderby prog.Prioridad ascending, prog.HoraInicioEstimada ascending
                select new ProgramacionCirugiaResponseDTO
                {
                    Codigo               = prog.Codigo,
                    CodigoPaciente       = pac.Codigo,
                    NombrePaciente       = pac.Nombre,
                    CodigoSala           = sala.Codigo,
                    NumeroSala           = sala.NumeroSala,
                    CodigoCirujano       = cir.Codigo,
                    NombreCirujano       = cir.Nombre,
                    FechaSolicitada      = prog.FechaSolicitada,
                    HoraInicioEstimada   = prog.HoraInicioEstimada,
                    HoraFinEstimada      = prog.HoraFinEstimada,
                    TipoCirugia          = prog.TipoCirugia,
                    Prioridad            = prog.Prioridad,
                    Observaciones        = prog.Observaciones,
                    EstadoProgramacion   = prog.EstadoProgramacion
                }
            ).ToListAsync();

            return Ok(resultado);
        }

        // ─────────────────────────────────────────────────────────────────────
        // POST /api/ProgramacionCirugia
        // Un médico programa una cirugía.
        // Estado inicial: "Solicitada"
        // ─────────────────────────────────────────────────────────────────────
        [HttpPost]
        public async Task<ActionResult> Create([FromQuery] ProgramacionCirugiaRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Codigo))
                return BadRequest(new { mensaje = "El campo 'codigo' es obligatorio." });

            if (await _context.ProgramacionesCirugia.AnyAsync(p => p.Codigo == dto.Codigo))
                return Conflict(new { mensaje = $"Ya existe una programación con código '{dto.Codigo}'." });

            // Validar paciente
            var paciente = await _context.Pacientes
                .FirstOrDefaultAsync(p => p.Codigo == dto.CodigoPaciente && p.Estado == "Activo");
            if (paciente is null)
                return NotFound(new { mensaje = $"Paciente '{dto.CodigoPaciente}' no encontrado o no activo." });

            // Validar sala
            var sala = await _context.SalasQuirurgicas
                .FirstOrDefaultAsync(s => s.Codigo == dto.CodigoSala && s.Estado == "Activo");
            if (sala is null)
                return NotFound(new { mensaje = $"Sala '{dto.CodigoSala}' no encontrada." });

            // Validar sala disponible
            if (!sala.Disponible)
                return BadRequest(new { mensaje = $"La sala '{dto.CodigoSala}' no está disponible actualmente." });

            // Validar cirujano
            var cirujano = await _context.Cirujanos
                .FirstOrDefaultAsync(c => c.Codigo == dto.CodigoCirujanoSolicitante && c.Estado == "Activo");
            if (cirujano is null)
                return NotFound(new { mensaje = $"Cirujano '{dto.CodigoCirujanoSolicitante}' no encontrado." });

            // Validar prioridad
            var prioridadesValidas = new[] { "Alta", "Media", "Baja" };
            if (!prioridadesValidas.Contains(dto.Prioridad))
                return BadRequest(new { mensaje = "El campo 'Prioridad' debe ser: Alta, Media o Baja." });

            _context.ProgramacionesCirugia.Add(new ProgramacionCirugia
            {
                Codigo                = dto.Codigo,
                PacienteId            = paciente.Id,
                SalaId                = sala.Id,
                CirujanoSolicitanteId = cirujano.Id,
                FechaSolicitada       = DateTime.UtcNow,
                HoraInicioEstimada    = dto.HoraInicioEstimada,
                HoraFinEstimada       = dto.HoraFinEstimada,
                TipoCirugia           = dto.TipoCirugia,
                Prioridad             = dto.Prioridad,
                Observaciones         = dto.Observaciones,
                EstadoProgramacion    = "Solicitada",
                Estado                = "Activo"
            });

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByCodigo), new { codigo = dto.Codigo }, new
            {
                mensaje              = "Cirugía programada correctamente. Estado: Solicitada (pendiente de aprobación).",
                codigo               = dto.Codigo,
                paciente             = paciente.Nombre,
                sala                 = sala.NumeroSala,
                cirujano             = cirujano.Nombre,
                horaInicioEstimada   = dto.HoraInicioEstimada,
                estadoProgramacion   = "Solicitada"
            });
        }

        // ─────────────────────────────────────────────────────────────────────
        // PUT /api/ProgramacionCirugia/{codigo}/estado
        // Jefatura quirúrgica aprueba o rechaza la programación.
        // ─────────────────────────────────────────────────────────────────────
        [HttpPut("{codigo}/estado")]
        public async Task<ActionResult> CambiarEstado(string codigo, [FromQuery] CambioEstadoProgramacionDTO dto)
        {
            var prog = await _context.ProgramacionesCirugia
                .FirstOrDefaultAsync(p => p.Codigo == codigo && p.Estado == "Activo");

            if (prog is null)
                return NotFound(new { mensaje = $"Programación '{codigo}' no encontrada." });

            var estadosValidos = new[] { "Aprobada", "Rechazada" };
            if (!estadosValidos.Contains(dto.NuevoEstado))
                return BadRequest(new { mensaje = "El campo 'NuevoEstado' debe ser 'Aprobada' o 'Rechazada'." });

            prog.EstadoProgramacion = dto.NuevoEstado;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje            = $"Programación '{codigo}' actualizada a estado '{dto.NuevoEstado}'.",
                codigo             = codigo,
                estadoProgramacion = dto.NuevoEstado,
                observaciones      = dto.Observaciones
            });
        }

        // ─────────────────────────────────────────────────────────────────────
        // POST /api/ProgramacionCirugia/{codigo}/ejecutar
        // Convierte una programación aprobada en un ProcedimientoQuirurgico real.
        // ─────────────────────────────────────────────────────────────────────
        [HttpPost("{codigo}/ejecutar")]
        public async Task<ActionResult> EjecutarProgramacion(string codigo)
        {
            var prog = await _context.ProgramacionesCirugia
                .FirstOrDefaultAsync(p => p.Codigo == codigo && p.Estado == "Activo");

            if (prog is null)
                return NotFound(new { mensaje = $"Programación '{codigo}' no encontrada." });

            if (prog.EstadoProgramacion != "Aprobada")
                return BadRequest(new
                {
                    mensaje = $"Solo se pueden ejecutar programaciones aprobadas. Estado actual: '{prog.EstadoProgramacion}'."
                });

            // Generar código para el procedimiento
            var codigoProc = $"PROC-{codigo}";
            if (await _context.ProcedimientosQuirurgicos.AnyAsync(p => p.Codigo == codigoProc))
                return Conflict(new { mensaje = $"Ya existe un procedimiento generado desde esta programación ({codigoProc})." });

            // Crear el ProcedimientoQuirurgico
            _context.ProcedimientosQuirurgicos.Add(new ProcedimientoQuirurgico
            {
                Codigo              = codigoProc,
                PacienteId          = prog.PacienteId,
                SalaId              = prog.SalaId,
                HoraInicio          = prog.HoraInicioEstimada,
                HoraFin             = prog.HoraFinEstimada,
                EstadoProcedimiento = "Programado",
                Prioridad           = prog.Prioridad,
                Estado              = "Activo"
            });

            // Marcar la programación como ejecutada
            prog.EstadoProgramacion = "Ejecutada";

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje             = "Programación ejecutada. Procedimiento quirúrgico creado correctamente.",
                codigoProgramacion  = codigo,
                codigoProcedimiento = codigoProc,
                estadoProcedimiento = "Programado"
            });
        }

        // ─────────────────────────────────────────────────────────────────────
        // DELETE /api/ProgramacionCirugia/{codigo} (Soft Delete)
        // ─────────────────────────────────────────────────────────────────────
        [HttpDelete("{codigo}")]
        public async Task<ActionResult> Delete(string codigo)
        {
            var prog = await _context.ProgramacionesCirugia
                .FirstOrDefaultAsync(p => p.Codigo == codigo && p.Estado == "Activo");

            if (prog is null)
                return NotFound(new { mensaje = $"Programación '{codigo}' no encontrada." });

            if (prog.EstadoProgramacion == "Ejecutada")
                return BadRequest(new { mensaje = "No se puede eliminar una programación ya ejecutada." });

            prog.Estado = "Inactivo";
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = $"Programación '{codigo}' cancelada (Soft Delete)." });
        }
    }
}
