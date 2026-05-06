namespace Gestion_Quirurgica.DTOs
{
    public class ProcedimientoPacienteSalaDTO
    {
        public string CodigoProcedimiento { get; set; } = string.Empty;
        public string NombrePaciente { get; set; } = string.Empty;
        public string GrupoSanguineo { get; set; } = string.Empty;
        public string NumeroSala { get; set; } = string.Empty;
        public string NivelEsterilidad { get; set; } = string.Empty;
        public DateTime HoraInicio { get; set; }
        public DateTime HoraFin { get; set; }
        public string EstadoProcedimiento { get; set; } = string.Empty;
        public string Prioridad { get; set; } = string.Empty;
    }

    public class ConteoEstadoDTO
    {
        public string EstadoProcedimiento { get; set; } = string.Empty;
        public int TotalProcedimientos { get; set; }
    }

    public class ExperienciaCirujanoDTO
    {
        public string Especialidad { get; set; } = string.Empty;
        public int TotalCirujanos { get; set; }
        public int SumaAnosExperiencia { get; set; }
        public double PromedioAnosExperiencia { get; set; }
    }

    public class DetalleProcedimientoDTO
    {
        public DetalleCabeceraDTO Cabecera { get; set; } = new();
        public List<DetalleCirujanoDTO> Cirujanos { get; set; } = new();
        public List<DetalleEquipoDTO> Equipos { get; set; } = new();
    }

    public class DetalleCabeceraDTO
    {
        public string CodigoProcedimiento { get; set; } = string.Empty;
        public string NombrePaciente { get; set; } = string.Empty;
        public string CodigoPaciente { get; set; } = string.Empty;
        public string DiagnosticoPre { get; set; } = string.Empty;
        public string GrupoSanguineo { get; set; } = string.Empty;
        public string NumeroSala { get; set; } = string.Empty;
        public string NivelEsterilidad { get; set; } = string.Empty;
        public DateTime HoraInicio { get; set; }
        public DateTime HoraFin { get; set; }
        public string EstadoProcedimiento { get; set; } = string.Empty;
        public string Prioridad { get; set; } = string.Empty;
    }

    public class DetalleCirujanoDTO
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Especialidad { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
    }

    public class DetalleEquipoDTO
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Fabricante { get; set; } = string.Empty;
        public string EstadoUso { get; set; } = string.Empty;
    }

    public class CirujanoSinProcedimientoDTO
    {
        public string CodigoCirujano { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Especialidad { get; set; } = string.Empty;
        public int AnosExperiencia { get; set; }
    }

    public class CirugiaElectivaDTO
    {
        public string CodigoProcedimiento { get; set; } = string.Empty;
        public string Paciente { get; set; } = string.Empty;
        public string DiagnosticoPre { get; set; } = string.Empty;
        public string Sala { get; set; } = string.Empty;
        public DateTime HoraInicio { get; set; }
        public string Prioridad { get; set; } = string.Empty;
    }

    public class EntradaQuirofanoDTO
    {
        public string CodigoEvento { get; set; } = string.Empty;
        public string CodigoProcedimiento { get; set; } = string.Empty;
        public string Paciente { get; set; } = string.Empty;
        public DateTime HoraEntrada { get; set; }
        public string UsuarioRegistro { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
    }

    public class ChecklistPendienteDTO
    {
        public string CodigoProcedimiento { get; set; } = string.Empty;
        public string Paciente { get; set; } = string.Empty;
        public string Sala { get; set; } = string.Empty;
        public DateTime HoraInicio { get; set; }
        public string EstadoProcedimiento { get; set; } = string.Empty;
        public string Alerta { get; set; } = string.Empty;
    }

    public class ProcedimientoEnCursoDTO
    {
        public string CodigoProcedimiento { get; set; } = string.Empty;
        public string Paciente { get; set; } = string.Empty;
        public string GrupoSanguineo { get; set; } = string.Empty;
        public string Sala { get; set; } = string.Empty;
        public string NivelEsterilidad { get; set; } = string.Empty;
        public DateTime HoraInicio { get; set; }
        public DateTime HoraFinProgramada { get; set; }
        public string Prioridad { get; set; } = string.Empty;
    }

    public class IncidenciaProcedimientoDTO
    {
        public string CodigoProcedimiento { get; set; } = string.Empty;
        public string Paciente { get; set; } = string.Empty;
        public string TipoEvento { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string UsuarioRegistra { get; set; } = string.Empty;
    }

    public class MaterialReservadoDTO
    {
        public string CodigoAsignacion { get; set; } = string.Empty;
        public string CodigoEquipo { get; set; } = string.Empty;
        public string NombreEquipo { get; set; } = string.Empty;
        public string Fabricante { get; set; } = string.Empty;
        public string CodigoProcedimiento { get; set; } = string.Empty;
        public string Paciente { get; set; } = string.Empty;
        public DateTime HoraInicio { get; set; }
        public string Observaciones { get; set; } = string.Empty;
    }

    public class CirugiaFinalizadaDTO
    {
        public string CodigoProcedimiento { get; set; } = string.Empty;
        public string Paciente { get; set; } = string.Empty;
        public string Sala { get; set; } = string.Empty;
        public DateTime HoraInicio { get; set; }
        public DateTime HoraFin { get; set; }
        public int DuracionMinutos { get; set; }
        public string Prioridad { get; set; } = string.Empty;
    }

    public class ReporteSalaDTO
    {
        public string CodigoSala { get; set; } = string.Empty;
        public string NumeroSala { get; set; } = string.Empty;
        public string NivelEsterilidad { get; set; } = string.Empty;
        public bool Disponible { get; set; }
        public DateTime UltimaLimpieza { get; set; }
        public int TotalProcedimientos { get; set; }
    }

    public class RolAsignadoDTO
    {
        public string CodigoProcedimiento { get; set; } = string.Empty;
        public string Paciente { get; set; } = string.Empty;
        public DateTime HoraInicio { get; set; }
        public string CodigoCirujano { get; set; } = string.Empty;
        public string NombreCirujano { get; set; } = string.Empty;
        public string Especialidad { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
    }

    public class DisponibilidadSalaDTO
    {
        public string CodigoSala { get; set; } = string.Empty;
        public string NumeroSala { get; set; } = string.Empty;
        public string NivelEsterilidad { get; set; } = string.Empty;
        public bool Disponible { get; set; }
        public DateTime UltimaLimpieza { get; set; }
        public bool TieneProcEnCurso { get; set; }
    }

    public class CirugiaUrgenciaDTO
    {
        public string CodigoProcedimiento { get; set; } = string.Empty;
        public string Paciente { get; set; } = string.Empty;
        public string DiagnosticoPre { get; set; } = string.Empty;
        public string GrupoSanguineo { get; set; } = string.Empty;
        public string Sala { get; set; } = string.Empty;
        public DateTime HoraInicio { get; set; }
        public string EstadoProcedimiento { get; set; } = string.Empty;
    }

    public class HistorialClinicoDTO
    {
        public HistorialPacienteDTO Paciente { get; set; } = new();
        public List<HistorialProcedimientoDTO> Procedimientos { get; set; } = new();
        public int Total { get; set; }
    }

    public class HistorialPacienteDTO
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public int Edad { get; set; }
        public string DiagnosticoPre { get; set; } = string.Empty;
        public string Alergias { get; set; } = string.Empty;
        public string GrupoSanguineo { get; set; } = string.Empty;
    }

    public class HistorialProcedimientoDTO
    {
        public string CodigoProcedimiento { get; set; } = string.Empty;
        public string Sala { get; set; } = string.Empty;
        public DateTime HoraInicio { get; set; }
        public DateTime HoraFin { get; set; }
        public string EstadoProcedimiento { get; set; } = string.Empty;
        public string Prioridad { get; set; } = string.Empty;
    }

    public class EquipoVencidoGrupoDTO
    {
        public string Fabricante { get; set; } = string.Empty;
        public int TotalEquiposVencidos { get; set; }
        public List<EquipoVencidoDetalleDTO> Equipos { get; set; } = new();
    }

    public class EquipoVencidoDetalleDTO
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public DateTime UltimoMantenimiento { get; set; }
        public bool EstadoOperativo { get; set; }
    }

    public class TiempoQuirurgicoDTO
    {
        public string CodigoProcedimiento { get; set; } = string.Empty;
        public string Paciente { get; set; } = string.Empty;
        public string Sala { get; set; } = string.Empty;
        public DateTime HoraInicio { get; set; }
        public DateTime HoraFinProgramada { get; set; }
        public string EstadoProcedimiento { get; set; } = string.Empty;
        public int DuracionProgramadaMin { get; set; }
        public string Prioridad { get; set; } = string.Empty;
    }

    public class ResumenEjecutivoDTO
    {
        public int TotalProcedimientos { get; set; }
        public int EnCurso { get; set; }
        public int Programados { get; set; }
        public int Finalizados { get; set; }
        public int TotalCirujanosActivos { get; set; }
        public int SalasDisponibles { get; set; }
        public int EquiposOperativos { get; set; }
        public int IncidenciasRegistradas { get; set; }
        public List<ConteoEstadoDTO> ProcedimientosPorPrioridad { get; set; } = new();
    }
}
