using Gestion_Quirurgica.Data;
using Gestion_Quirurgica.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gestion_Quirurgica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MisController : ControllerBase
    {
        private readonly GestionQuirurgicaContext _ctx;
        public MisController(GestionQuirurgicaContext ctx) => _ctx = ctx;

        [HttpGet("uc01-cirugia-electiva-programada")]
        public async Task<ActionResult<IEnumerable<CirugiaElectivaDTO>>> UC01()
        {
            var resultado = await (
                from proc in _ctx.ProcedimientosQuirurgicos
                where proc.Estado == "Activo" && proc.EstadoProcedimiento == "Programado"
                join pac  in _ctx.Pacientes        on proc.PacienteId equals pac.Id
                join sala in _ctx.SalasQuirurgicas on proc.SalaId     equals sala.Id
                orderby proc.Prioridad ascending, proc.HoraInicio ascending
                select new CirugiaElectivaDTO
                {
                    CodigoProcedimiento = proc.Codigo,
                    Paciente            = pac.Nombre,
                    DiagnosticoPre      = pac.DiagnosticoPre,
                    Sala                = sala.NumeroSala,
                    HoraInicio          = proc.HoraInicio,
                    Prioridad           = proc.Prioridad
                }
            ).ToListAsync();

            return Ok(resultado);
        }

        [HttpGet("uc02-entradas-a-quirofano")]
        public async Task<ActionResult<IEnumerable<EntradaQuirofanoDTO>>> UC02()
        {
            var resultado = await (
                from tl   in _ctx.TimelineIncidencias
                where tl.Estado == "Activo" && tl.Tipo == "Inicio"
                join proc in _ctx.ProcedimientosQuirurgicos on tl.ProcedimientoId equals proc.Id
                where proc.Estado == "Activo"
                join pac  in _ctx.Pacientes on proc.PacienteId equals pac.Id
                orderby tl.Timestamp descending
                select new EntradaQuirofanoDTO
                {
                    CodigoEvento        = tl.Codigo,
                    CodigoProcedimiento = proc.Codigo,
                    Paciente            = pac.Nombre,
                    HoraEntrada         = tl.Timestamp,
                    UsuarioRegistro     = tl.UsuarioRegistra,
                    Descripcion         = tl.Descripcion
                }
            ).ToListAsync();

            return Ok(resultado);
        }

        [HttpGet("uc03-checklist-seguridad-pendiente")]
        public async Task<ActionResult<IEnumerable<ChecklistPendienteDTO>>> UC03()
        {
            var resultado = await (
                from proc in _ctx.ProcedimientosQuirurgicos
                where proc.Estado == "Activo"
                where !_ctx.TimelineIncidencias.Any(tl =>
                    tl.ProcedimientoId == proc.Id &&
                    tl.Tipo == "Inicio" &&
                    tl.Estado == "Activo")
                join pac  in _ctx.Pacientes        on proc.PacienteId equals pac.Id
                join sala in _ctx.SalasQuirurgicas on proc.SalaId     equals sala.Id
                select new ChecklistPendienteDTO
                {
                    CodigoProcedimiento = proc.Codigo,
                    Paciente            = pac.Nombre,
                    Sala                = sala.NumeroSala,
                    HoraInicio          = proc.HoraInicio,
                    EstadoProcedimiento = proc.EstadoProcedimiento,
                    Alerta              = "Sin checklist de entrada registrado"
                }
            ).ToListAsync();

            return Ok(resultado);
        }

        [HttpGet("uc04-procedimientos-en-curso")]
        public async Task<ActionResult<IEnumerable<ProcedimientoEnCursoDTO>>> UC04()
        {
            var resultado = await (
                from proc in _ctx.ProcedimientosQuirurgicos
                where proc.Estado == "Activo" && proc.EstadoProcedimiento == "En curso"
                join pac  in _ctx.Pacientes        on proc.PacienteId equals pac.Id
                join sala in _ctx.SalasQuirurgicas on proc.SalaId     equals sala.Id
                select new ProcedimientoEnCursoDTO
                {
                    CodigoProcedimiento = proc.Codigo,
                    Paciente            = pac.Nombre,
                    GrupoSanguineo      = pac.GrupoSanguineo,
                    Sala                = sala.NumeroSala,
                    NivelEsterilidad    = sala.NivelEsterilidad,
                    HoraInicio          = proc.HoraInicio,
                    HoraFinProgramada   = proc.HoraFin,
                    Prioridad           = proc.Prioridad
                }
            ).ToListAsync();

            return Ok(resultado);
        }

        [HttpGet("uc05-incidencias-por-procedimiento")]
        public async Task<ActionResult<IEnumerable<IncidenciaProcedimientoDTO>>> UC05()
        {
            var resultado = await (
                from tl   in _ctx.TimelineIncidencias
                where tl.Estado == "Activo"
                      && (tl.Tipo == "Incidencia" || tl.Tipo == "Observación")
                join proc in _ctx.ProcedimientosQuirurgicos on tl.ProcedimientoId equals proc.Id
                where proc.Estado == "Activo"
                join pac  in _ctx.Pacientes on proc.PacienteId equals pac.Id
                orderby proc.Codigo, tl.Timestamp
                select new IncidenciaProcedimientoDTO
                {
                    CodigoProcedimiento = proc.Codigo,
                    Paciente            = pac.Nombre,
                    TipoEvento          = tl.Tipo,
                    Timestamp           = tl.Timestamp,
                    Descripcion         = tl.Descripcion,
                    UsuarioRegistra     = tl.UsuarioRegistra
                }
            ).ToListAsync();

            return Ok(resultado);
        }

        [HttpGet("uc07-material-reservado")]
        public async Task<ActionResult<IEnumerable<MaterialReservadoDTO>>> UC07()
        {
            var resultado = await (
                from pe   in _ctx.ProcedimientosEquipo
                where pe.Estado == "Activo" && pe.EstadoUso == "Reservado"
                join proc in _ctx.ProcedimientosQuirurgicos on pe.ProcedimientoId equals proc.Id
                where proc.Estado == "Activo"
                join eq   in _ctx.EquiposMedicos on pe.EquipoId equals eq.Id
                where eq.Estado == "Activo"
                join pac  in _ctx.Pacientes on proc.PacienteId equals pac.Id
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

        [HttpGet("uc08-cirugias-finalizadas")]
        public async Task<ActionResult<IEnumerable<CirugiaFinalizadaDTO>>> UC08()
        {
            var datos = await (
                from proc in _ctx.ProcedimientosQuirurgicos
                where proc.Estado == "Activo" && proc.EstadoProcedimiento == "Finalizado"
                join pac  in _ctx.Pacientes        on proc.PacienteId equals pac.Id
                join sala in _ctx.SalasQuirurgicas on proc.SalaId     equals sala.Id
                orderby proc.HoraFin descending
                select new
                {
                    proc.Codigo,
                    Paciente   = pac.Nombre,
                    Sala       = sala.NumeroSala,
                    proc.HoraInicio,
                    proc.HoraFin,
                    proc.Prioridad
                }
            ).ToListAsync();

            var resultado = datos.Select(r => new CirugiaFinalizadaDTO
            {
                CodigoProcedimiento = r.Codigo,
                Paciente            = r.Paciente,
                Sala                = r.Sala,
                HoraInicio          = r.HoraInicio,
                HoraFin             = r.HoraFin,
                DuracionMinutos     = (int)(r.HoraFin - r.HoraInicio).TotalMinutes,
                Prioridad           = r.Prioridad
            }).ToList();

            return Ok(resultado);
        }

        [HttpGet("uc09-reporte-uso-por-sala")]
        public async Task<ActionResult<IEnumerable<ReporteSalaDTO>>> UC09()
        {
            var resultado = await (
                from sala in _ctx.SalasQuirurgicas
                where sala.Estado == "Activo"
                join proc in _ctx.ProcedimientosQuirurgicos
                    on sala.Id equals proc.SalaId into procGroup
                from pg in procGroup.DefaultIfEmpty()
                group new { sala, pg } by new
                {
                    sala.Codigo,
                    sala.NumeroSala,
                    sala.NivelEsterilidad,
                    sala.Disponible,
                    sala.UltimaLimpieza
                } into g
                select new ReporteSalaDTO
                {
                    CodigoSala          = g.Key.Codigo,
                    NumeroSala          = g.Key.NumeroSala,
                    NivelEsterilidad    = g.Key.NivelEsterilidad,
                    Disponible          = g.Key.Disponible,
                    UltimaLimpieza      = g.Key.UltimaLimpieza,
                    TotalProcedimientos = g.Count(x => x.pg != null && x.pg.Estado == "Activo")
                }
            ).ToListAsync();

            return Ok(resultado);
        }

        [HttpGet("uc10-roles-asignados-por-procedimiento")]
        public async Task<ActionResult<IEnumerable<RolAsignadoDTO>>> UC10()
        {
            var resultado = await (
                from rol  in _ctx.RolesQuirurgicos
                where rol.Estado == "Activo"
                join proc in _ctx.ProcedimientosQuirurgicos on rol.ProcedimientoId equals proc.Id
                where proc.Estado == "Activo"
                join cir  in _ctx.Cirujanos on rol.CirujanoId equals cir.Id
                where cir.Estado == "Activo"
                join pac  in _ctx.Pacientes on proc.PacienteId equals pac.Id
                orderby proc.Codigo, rol.Rol
                select new RolAsignadoDTO
                {
                    CodigoProcedimiento = proc.Codigo,
                    Paciente            = pac.Nombre,
                    HoraInicio          = proc.HoraInicio,
                    CodigoCirujano      = cir.Codigo,
                    NombreCirujano      = cir.Nombre,
                    Especialidad        = cir.Especialidad,
                    Rol                 = rol.Rol
                }
            ).ToListAsync();

            return Ok(resultado);
        }

        [HttpGet("uc11-disponibilidad-salas")]
        public async Task<ActionResult<IEnumerable<DisponibilidadSalaDTO>>> UC11()
        {
            var resultado = await (
                from sala in _ctx.SalasQuirurgicas
                where sala.Estado == "Activo"
                select new DisponibilidadSalaDTO
                {
                    CodigoSala       = sala.Codigo,
                    NumeroSala       = sala.NumeroSala,
                    NivelEsterilidad = sala.NivelEsterilidad,
                    Disponible       = sala.Disponible,
                    UltimaLimpieza   = sala.UltimaLimpieza,
                    TieneProcEnCurso = _ctx.ProcedimientosQuirurgicos.Any(p =>
                        p.SalaId == sala.Id &&
                        p.EstadoProcedimiento == "En curso" &&
                        p.Estado == "Activo")
                }
            ).ToListAsync();

            return Ok(resultado);
        }

        [HttpGet("uc12-cirugias-urgencia")]
        public async Task<ActionResult<IEnumerable<CirugiaUrgenciaDTO>>> UC12()
        {
            var resultado = await (
                from proc in _ctx.ProcedimientosQuirurgicos
                where proc.Estado == "Activo" && proc.Prioridad == "Alta"
                join pac  in _ctx.Pacientes        on proc.PacienteId equals pac.Id
                join sala in _ctx.SalasQuirurgicas on proc.SalaId     equals sala.Id
                orderby proc.HoraInicio ascending
                select new CirugiaUrgenciaDTO
                {
                    CodigoProcedimiento = proc.Codigo,
                    Paciente            = pac.Nombre,
                    DiagnosticoPre      = pac.DiagnosticoPre,
                    GrupoSanguineo      = pac.GrupoSanguineo,
                    Sala                = sala.NumeroSala,
                    HoraInicio          = proc.HoraInicio,
                    EstadoProcedimiento = proc.EstadoProcedimiento
                }
            ).ToListAsync();

            return Ok(resultado);
        }

        [HttpGet("uc13-historial-clinico/{codigoPaciente}")]
        public async Task<ActionResult<HistorialClinicoDTO>> UC13(string codigoPaciente)
        {
            var pacienteInfo = await (
                from p in _ctx.Pacientes
                where p.Codigo == codigoPaciente && p.Estado == "Activo"
                select new HistorialPacienteDTO
                {
                    Codigo         = p.Codigo,
                    Nombre         = p.Nombre,
                    Edad           = p.Edad,
                    DiagnosticoPre = p.DiagnosticoPre,
                    Alergias       = p.Alergias,
                    GrupoSanguineo = p.GrupoSanguineo
                }
            ).FirstOrDefaultAsync();

            if (pacienteInfo is null)
                return NotFound(new { mensaje = $"Paciente '{codigoPaciente}' no encontrado." });

            var procedimientos = await (
                from proc in _ctx.ProcedimientosQuirurgicos
                where proc.Estado == "Activo"
                join pac  in _ctx.Pacientes on proc.PacienteId equals pac.Id
                where pac.Codigo == codigoPaciente
                join sala in _ctx.SalasQuirurgicas on proc.SalaId equals sala.Id
                orderby proc.HoraInicio descending
                select new HistorialProcedimientoDTO
                {
                    CodigoProcedimiento = proc.Codigo,
                    Sala                = sala.NumeroSala,
                    HoraInicio          = proc.HoraInicio,
                    HoraFin             = proc.HoraFin,
                    EstadoProcedimiento = proc.EstadoProcedimiento,
                    Prioridad           = proc.Prioridad
                }
            ).ToListAsync();

            return Ok(new HistorialClinicoDTO
            {
                Paciente       = pacienteInfo,
                Procedimientos = procedimientos,
                Total          = procedimientos.Count
            });
        }

        [HttpGet("uc14-equipos-mantenimiento-vencido")]
        public async Task<ActionResult<IEnumerable<EquipoVencidoGrupoDTO>>> UC14()
        {
            var fechaLimite = DateTime.UtcNow.AddDays(-30);

            var datos = await (
                from eq in _ctx.EquiposMedicos
                where eq.Estado == "Activo" && eq.UltimoMantenimiento < fechaLimite
                group eq by eq.Fabricante into g
                select new EquipoVencidoGrupoDTO
                {
                    Fabricante           = g.Key,
                    TotalEquiposVencidos = g.Count(),
                    Equipos = g.Select(e => new EquipoVencidoDetalleDTO
                    {
                        Codigo              = e.Codigo,
                        Nombre              = e.Nombre,
                        UltimoMantenimiento = e.UltimoMantenimiento,
                        EstadoOperativo     = e.EstadoOperativo
                    }).ToList()
                }
            ).ToListAsync();

            return Ok(datos);
        }

        [HttpGet("uc15-control-tiempos-quirurgicos")]
        public async Task<ActionResult<IEnumerable<TiempoQuirurgicoDTO>>> UC15()
        {
            var datos = await (
                from proc in _ctx.ProcedimientosQuirurgicos
                where proc.Estado == "Activo"
                join pac  in _ctx.Pacientes        on proc.PacienteId equals pac.Id
                join sala in _ctx.SalasQuirurgicas on proc.SalaId     equals sala.Id
                select new
                {
                    proc.Codigo,
                    Paciente            = pac.Nombre,
                    Sala                = sala.NumeroSala,
                    proc.HoraInicio,
                    proc.HoraFin,
                    proc.EstadoProcedimiento,
                    proc.Prioridad
                }
            ).ToListAsync();

            var resultado = datos.Select(r => new TiempoQuirurgicoDTO
            {
                CodigoProcedimiento   = r.Codigo,
                Paciente              = r.Paciente,
                Sala                  = r.Sala,
                HoraInicio            = r.HoraInicio,
                HoraFinProgramada     = r.HoraFin,
                EstadoProcedimiento   = r.EstadoProcedimiento,
                DuracionProgramadaMin = (int)(r.HoraFin - r.HoraInicio).TotalMinutes,
                Prioridad             = r.Prioridad
            }).ToList();

            return Ok(resultado);
        }

        [HttpGet("resumen-ejecutivo")]
        public async Task<ActionResult<ResumenEjecutivoDTO>> ResumenEjecutivo()
        {
            var totalProcedimientos   = await _ctx.ProcedimientosQuirurgicos.CountAsync(p => p.Estado == "Activo");
            var totalEnCurso          = await _ctx.ProcedimientosQuirurgicos.CountAsync(p => p.Estado == "Activo" && p.EstadoProcedimiento == "En curso");
            var totalProgramados      = await _ctx.ProcedimientosQuirurgicos.CountAsync(p => p.Estado == "Activo" && p.EstadoProcedimiento == "Programado");
            var totalFinalizados      = await _ctx.ProcedimientosQuirurgicos.CountAsync(p => p.Estado == "Activo" && p.EstadoProcedimiento == "Finalizado");
            var totalCirujanos        = await _ctx.Cirujanos.CountAsync(c => c.Estado == "Activo");
            var salasDisponibles      = await _ctx.SalasQuirurgicas.CountAsync(s => s.Estado == "Activo" && s.Disponible);
            var equiposOperativos     = await _ctx.EquiposMedicos.CountAsync(e => e.Estado == "Activo" && e.EstadoOperativo);
            var totalIncidencias      = await _ctx.TimelineIncidencias.CountAsync(t => t.Estado == "Activo" && (t.Tipo == "Incidencia" || t.Tipo == "Observación"));

            var porPrioridad = await (
                from proc in _ctx.ProcedimientosQuirurgicos
                where proc.Estado == "Activo"
                group proc by proc.Prioridad into g
                select new ConteoEstadoDTO
                {
                    EstadoProcedimiento = g.Key,
                    TotalProcedimientos = g.Count()
                }
            ).ToListAsync();

            return Ok(new ResumenEjecutivoDTO
            {
                TotalProcedimientos        = totalProcedimientos,
                EnCurso                    = totalEnCurso,
                Programados                = totalProgramados,
                Finalizados                = totalFinalizados,
                TotalCirujanosActivos      = totalCirujanos,
                SalasDisponibles           = salasDisponibles,
                EquiposOperativos          = equiposOperativos,
                IncidenciasRegistradas     = totalIncidencias,
                ProcedimientosPorPrioridad = porPrioridad
            });
        }
    }
}
