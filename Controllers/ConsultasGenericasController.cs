using Gestion_Quirurgica.Data;
using Gestion_Quirurgica.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gestion_Quirurgica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsultasGenericasController : ControllerBase
    {
        private readonly GestionQuirurgicaContext _ctx;
        public ConsultasGenericasController(GestionQuirurgicaContext ctx) => _ctx = ctx;

        [HttpGet("cg01-listado-procedimientos-paciente-sala")]
        public async Task<ActionResult<IEnumerable<ProcedimientoPacienteSalaDTO>>> CG01()
        {
            var resultado = await (
                from proc in _ctx.ProcedimientosQuirurgicos
                where proc.Estado == "Activo"
                join pac  in _ctx.Pacientes        on proc.PacienteId equals pac.Id
                where pac.Estado == "Activo"
                join sala in _ctx.SalasQuirurgicas on proc.SalaId     equals sala.Id
                where sala.Estado == "Activo"
                select new ProcedimientoPacienteSalaDTO
                {
                    CodigoProcedimiento = proc.Codigo,
                    NombrePaciente      = pac.Nombre,
                    GrupoSanguineo      = pac.GrupoSanguineo,
                    NumeroSala          = sala.NumeroSala,
                    NivelEsterilidad    = sala.NivelEsterilidad,
                    HoraInicio          = proc.HoraInicio,
                    HoraFin             = proc.HoraFin,
                    EstadoProcedimiento = proc.EstadoProcedimiento,
                    Prioridad           = proc.Prioridad
                }
            ).ToListAsync();

            return Ok(resultado);
        }

        [HttpGet("cg02-count-procedimientos-por-estado")]
        public async Task<ActionResult<IEnumerable<ConteoEstadoDTO>>> CG02()
        {
            var resultado = await (
                from proc in _ctx.ProcedimientosQuirurgicos
                where proc.Estado == "Activo"
                group proc by proc.EstadoProcedimiento into g
                select new ConteoEstadoDTO
                {
                    EstadoProcedimiento = g.Key,
                    TotalProcedimientos = g.Count()
                }
            ).ToListAsync();

            return Ok(resultado);
        }

        [HttpGet("cg03-promedio-experiencia-por-especialidad")]
        public async Task<ActionResult<IEnumerable<ExperienciaCirujanoDTO>>> CG03()
        {
            var resultado = await (
                from c in _ctx.Cirujanos
                where c.Estado == "Activo"
                group c by c.Especialidad into g
                select new ExperienciaCirujanoDTO
                {
                    Especialidad            = g.Key,
                    TotalCirujanos          = g.Count(),
                    SumaAnosExperiencia     = g.Sum(c => c.AnosExperiencia),
                    PromedioAnosExperiencia = g.Average(c => c.AnosExperiencia)
                }
            ).ToListAsync();

            return Ok(resultado);
        }

        [HttpGet("cg04-detalle-por-codigo/{codigoProcedimiento}")]
        public async Task<ActionResult<DetalleProcedimientoDTO>> CG04(string codigoProcedimiento)
        {
            var cabecera = await (
                from proc in _ctx.ProcedimientosQuirurgicos
                where proc.Codigo == codigoProcedimiento && proc.Estado == "Activo"
                join pac  in _ctx.Pacientes        on proc.PacienteId equals pac.Id
                join sala in _ctx.SalasQuirurgicas on proc.SalaId     equals sala.Id
                select new DetalleCabeceraDTO
                {
                    CodigoProcedimiento = proc.Codigo,
                    NombrePaciente      = pac.Nombre,
                    CodigoPaciente      = pac.Codigo,
                    DiagnosticoPre      = pac.DiagnosticoPre,
                    GrupoSanguineo      = pac.GrupoSanguineo,
                    NumeroSala          = sala.NumeroSala,
                    NivelEsterilidad    = sala.NivelEsterilidad,
                    HoraInicio          = proc.HoraInicio,
                    HoraFin             = proc.HoraFin,
                    EstadoProcedimiento = proc.EstadoProcedimiento,
                    Prioridad           = proc.Prioridad
                }
            ).FirstOrDefaultAsync();

            if (cabecera is null)
                return NotFound(new { mensaje = $"Procedimiento '{codigoProcedimiento}' no encontrado." });

            var cirujanos = await (
                from rol  in _ctx.RolesQuirurgicos
                where rol.Estado == "Activo"
                join proc in _ctx.ProcedimientosQuirurgicos on rol.ProcedimientoId equals proc.Id
                where proc.Codigo == codigoProcedimiento
                join cir  in _ctx.Cirujanos on rol.CirujanoId equals cir.Id
                where cir.Estado == "Activo"
                select new DetalleCirujanoDTO
                {
                    Codigo       = cir.Codigo,
                    Nombre       = cir.Nombre,
                    Especialidad = cir.Especialidad,
                    Rol          = rol.Rol
                }
            ).ToListAsync();

            var equipos = await (
                from pe   in _ctx.ProcedimientosEquipo
                where pe.Estado == "Activo"
                join proc in _ctx.ProcedimientosQuirurgicos on pe.ProcedimientoId equals proc.Id
                where proc.Codigo == codigoProcedimiento
                join eq   in _ctx.EquiposMedicos on pe.EquipoId equals eq.Id
                where eq.Estado == "Activo"
                select new DetalleEquipoDTO
                {
                    Codigo     = eq.Codigo,
                    Nombre     = eq.Nombre,
                    Fabricante = eq.Fabricante,
                    EstadoUso  = pe.EstadoUso
                }
            ).ToListAsync();

            return Ok(new DetalleProcedimientoDTO
            {
                Cabecera  = cabecera,
                Cirujanos = cirujanos,
                Equipos   = equipos
            });
        }

        [HttpGet("cg05-cirujanos-sin-procedimiento")]
        public async Task<ActionResult<IEnumerable<CirujanoSinProcedimientoDTO>>> CG05()
        {
            var resultado = await (
                from cir in _ctx.Cirujanos
                where cir.Estado == "Activo"
                where !_ctx.RolesQuirurgicos.Any(r =>
                    r.CirujanoId == cir.Id &&
                    r.Estado == "Activo" &&
                    _ctx.ProcedimientosQuirurgicos.Any(p =>
                        p.Id == r.ProcedimientoId && p.Estado == "Activo"))
                select new CirujanoSinProcedimientoDTO
                {
                    CodigoCirujano  = cir.Codigo,
                    Nombre          = cir.Nombre,
                    Especialidad    = cir.Especialidad,
                    AnosExperiencia = cir.AnosExperiencia
                }
            ).ToListAsync();

            return Ok(resultado);
        }
    }
}
