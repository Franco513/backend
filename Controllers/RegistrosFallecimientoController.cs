using Gestion_Quirurgica.Data;
using Gestion_Quirurgica.Dominio;
using Gestion_Quirurgica.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gestion_Quirurgica.Controllers
{
    /// <summary>
    /// Controlador de Registros de Fallecimiento.
    ///
    /// INTEGRACIÓN CON GESTIÓN LEGAL:
    ///   - POST /api/RegistrosFallecimiento  → Gestión Quirúrgica registra el fallecimiento
    ///     cuando ocurre en quirófano. El registro queda en estado "Pendiente".
    ///
    ///   - GET /api/RegistrosFallecimiento/pendientes  → Gestión Legal puede consultar 
    ///     todos los fallecimientos pendientes de legalizar.
    ///
    ///   - POST /api/RegistrosFallecimiento/{codigo}/legalizacion  → Gestión Legal llama
    ///     este endpoint para adjuntar el número de acta y confirmar la legalización.
    ///     Cambia el estado del paciente a "Fallecido".
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RegistrosFallecimientoController : ControllerBase
    {
        private readonly GestionQuirurgicaContext _context;
        public RegistrosFallecimientoController(GestionQuirurgicaContext context) => _context = context;

        // ─────────────────────────────────────────────────────────────────────
        // GET /api/RegistrosFallecimiento
        // Devuelve todos los registros de fallecimiento activos.
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RegistroFallecimientoResponseDTO>>> GetAll()
        {
            var resultado = await (
                from rf   in _context.RegistrosFallecimiento
                where rf.Estado == "Activo"
                join proc in _context.ProcedimientosQuirurgicos on rf.ProcedimientoId equals proc.Id
                join pac  in _context.Pacientes on proc.PacienteId equals pac.Id
                orderby rf.FechaHoraFallecimiento descending
                select new RegistroFallecimientoResponseDTO
                {
                    Codigo                  = rf.Codigo,
                    CodigoProcedimiento     = proc.Codigo,
                    NombrePaciente          = pac.Nombre,
                    CodigoPaciente          = pac.Codigo,
                    FechaHoraFallecimiento  = rf.FechaHoraFallecimiento,
                    CausaFallecimiento      = rf.CausaFallecimiento,
                    UsuarioRegistra         = rf.UsuarioRegistra,
                    NumeroActaLegal         = rf.NumeroActaLegal,
                    EstadoLegal             = rf.EstadoLegal
                }
            ).ToListAsync();

            return Ok(resultado);
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET /api/RegistrosFallecimiento/pendientes
        // Gestión Legal usa este endpoint para ver qué casos debe legalizar.
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet("pendientes")]
        public async Task<ActionResult<IEnumerable<RegistroFallecimientoResponseDTO>>> GetPendientes()
        {
            var resultado = await (
                from rf   in _context.RegistrosFallecimiento
                where rf.Estado == "Activo" && rf.EstadoLegal == "Pendiente"
                join proc in _context.ProcedimientosQuirurgicos on rf.ProcedimientoId equals proc.Id
                join pac  in _context.Pacientes on proc.PacienteId equals pac.Id
                orderby rf.FechaHoraFallecimiento ascending
                select new RegistroFallecimientoResponseDTO
                {
                    Codigo                  = rf.Codigo,
                    CodigoProcedimiento     = proc.Codigo,
                    NombrePaciente          = pac.Nombre,
                    CodigoPaciente          = pac.Codigo,
                    FechaHoraFallecimiento  = rf.FechaHoraFallecimiento,
                    CausaFallecimiento      = rf.CausaFallecimiento,
                    UsuarioRegistra         = rf.UsuarioRegistra,
                    NumeroActaLegal         = rf.NumeroActaLegal,
                    EstadoLegal             = rf.EstadoLegal
                }
            ).ToListAsync();

            return Ok(resultado);
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET /api/RegistrosFallecimiento/{codigo}
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet("{codigo}")]
        public async Task<ActionResult<RegistroFallecimientoResponseDTO>> GetByCodigo(string codigo)
        {
            var resultado = await (
                from rf   in _context.RegistrosFallecimiento
                where rf.Codigo == codigo && rf.Estado == "Activo"
                join proc in _context.ProcedimientosQuirurgicos on rf.ProcedimientoId equals proc.Id
                join pac  in _context.Pacientes on proc.PacienteId equals pac.Id
                select new RegistroFallecimientoResponseDTO
                {
                    Codigo                  = rf.Codigo,
                    CodigoProcedimiento     = proc.Codigo,
                    NombrePaciente          = pac.Nombre,
                    CodigoPaciente          = pac.Codigo,
                    FechaHoraFallecimiento  = rf.FechaHoraFallecimiento,
                    CausaFallecimiento      = rf.CausaFallecimiento,
                    UsuarioRegistra         = rf.UsuarioRegistra,
                    NumeroActaLegal         = rf.NumeroActaLegal,
                    EstadoLegal             = rf.EstadoLegal
                }
            ).FirstOrDefaultAsync();

            if (resultado is null)
                return NotFound(new { mensaje = $"Registro de fallecimiento '{codigo}' no encontrado." });

            return Ok(resultado);
        }

        // ─────────────────────────────────────────────────────────────────────
        // POST /api/RegistrosFallecimiento
        // Gestión Quirúrgica registra el fallecimiento de un paciente en quirófano.
        // Automáticamente:
        //   1. Crea el registro con EstadoLegal = "Pendiente"
        //   2. Marca el procedimiento como "Finalizado"
        //   3. Marca el paciente como "Fallecido"
        // ─────────────────────────────────────────────────────────────────────
        [HttpPost]
        public async Task<ActionResult> Create([FromQuery] RegistroFallecimientoRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Codigo))
                return BadRequest(new { mensaje = "El campo 'codigo' es obligatorio." });

            if (await _context.RegistrosFallecimiento.AnyAsync(r => r.Codigo == dto.Codigo))
                return Conflict(new { mensaje = $"Ya existe un registro con código '{dto.Codigo}'." });

            // Buscar el procedimiento
            var procedimiento = await _context.ProcedimientosQuirurgicos
                .FirstOrDefaultAsync(p => p.Codigo == dto.CodigoProcedimiento && p.Estado == "Activo");
            if (procedimiento is null)
                return NotFound(new { mensaje = $"Procedimiento '{dto.CodigoProcedimiento}' no encontrado." });

            // Buscar el paciente vinculado al procedimiento
            var paciente = await _context.Pacientes
                .FirstOrDefaultAsync(p => p.Id == procedimiento.PacienteId);
            if (paciente is null)
                return NotFound(new { mensaje = "Paciente vinculado al procedimiento no encontrado." });

            // 1. Crear el registro de fallecimiento
            _context.RegistrosFallecimiento.Add(new RegistroFallecimiento
            {
                Codigo                 = dto.Codigo,
                ProcedimientoId        = procedimiento.Id,
                FechaHoraFallecimiento = dto.FechaHoraFallecimiento,
                CausaFallecimiento     = dto.CausaFallecimiento,
                UsuarioRegistra        = dto.UsuarioRegistra,
                EstadoLegal            = "Pendiente",
                Estado                 = "Activo"
            });

            // 2. Finalizar el procedimiento quirúrgico
            procedimiento.EstadoProcedimiento = "Finalizado";
            procedimiento.HoraFin             = dto.FechaHoraFallecimiento;

            // 3. Dar de baja al paciente (marcarlo como fallecido)
            paciente.Estado = "Fallecido";

            await _context.SaveChangesAsync();

            // Registrar evento en el timeline del procedimiento
            var codigoTimeline = $"TL-FALL-{dto.Codigo}";
            _context.TimelineIncidencias.Add(new TimelineIncidencia
            {
                Codigo          = codigoTimeline,
                ProcedimientoId = procedimiento.Id,
                Timestamp       = dto.FechaHoraFallecimiento,
                Tipo            = "Fallecimiento",
                Descripcion     = $"Fallecimiento registrado. Causa: {dto.CausaFallecimiento}. Pendiente legalización.",
                UsuarioRegistra = dto.UsuarioRegistra,
                Estado          = "Activo"
            });
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByCodigo), new { codigo = dto.Codigo }, new
            {
                mensaje              = "Fallecimiento registrado. Enviado a Gestión Legal para legalización.",
                codigoRegistro       = dto.Codigo,
                codigoProcedimiento  = dto.CodigoProcedimiento,
                pacienteDadoDeBaja   = paciente.Nombre,
                estadoLegal          = "Pendiente"
            });
        }

        // ─────────────────────────────────────────────────────────────────────
        // POST /api/RegistrosFallecimiento/{codigo}/legalizacion
        // ENDPOINT QUE LLAMA GESTIÓN LEGAL para adjuntar el número de acta
        // y confirmar (o rechazar) la legalización del fallecimiento.
        // ─────────────────────────────────────────────────────────────────────
        [HttpPost("{codigo}/legalizacion")]
        public async Task<ActionResult> AdjuntarLegalizacion(string codigo, [FromQuery] LegalizacionDTO dto)
        {
            var registro = await _context.RegistrosFallecimiento
                .FirstOrDefaultAsync(r => r.Codigo == codigo && r.Estado == "Activo");

            if (registro is null)
                return NotFound(new { mensaje = $"Registro de fallecimiento '{codigo}' no encontrado." });

            if (registro.EstadoLegal == "Legalizado")
                return Conflict(new { mensaje = $"El registro '{codigo}' ya fue legalizado con acta '{registro.NumeroActaLegal}'." });

            if (string.IsNullOrWhiteSpace(dto.NumeroActaLegal))
                return BadRequest(new { mensaje = "El campo 'NumeroActaLegal' es obligatorio." });

            if (dto.EstadoLegal != "Legalizado" && dto.EstadoLegal != "Rechazado")
                return BadRequest(new { mensaje = "El campo 'EstadoLegal' debe ser 'Legalizado' o 'Rechazado'." });

            registro.NumeroActaLegal = dto.NumeroActaLegal;
            registro.EstadoLegal     = dto.EstadoLegal;

            // Registrar en el timeline del procedimiento
            _context.TimelineIncidencias.Add(new TimelineIncidencia
            {
                Codigo          = $"TL-LEG-{codigo}",
                ProcedimientoId = registro.ProcedimientoId,
                Timestamp       = DateTime.UtcNow,
                Tipo            = "Legalización",
                Descripcion     = $"Gestión Legal procesó el caso. Estado: {dto.EstadoLegal}. Acta: {dto.NumeroActaLegal}. {dto.Observaciones}",
                UsuarioRegistra = "GestionLegal",
                Estado          = "Activo"
            });

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje         = $"Legalización procesada correctamente. Estado: {dto.EstadoLegal}.",
                codigoRegistro  = codigo,
                numeroActaLegal = dto.NumeroActaLegal,
                estadoLegal     = dto.EstadoLegal
            });
        }
    }
}
