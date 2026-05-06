namespace Gestion_Quirurgica.DTOs
{
    // ── Registro de Fallecimiento (integración con Gestión Legal) ─────────────

    public class RegistroFallecimientoRequestDTO
    {
        public string Codigo { get; set; } = string.Empty;

        /// <summary>Código del procedimiento quirúrgico en el que ocurrió</summary>
        public string CodigoProcedimiento { get; set; } = string.Empty;

        public DateTime FechaHoraFallecimiento { get; set; }

        public string CausaFallecimiento { get; set; } = string.Empty;

        public string UsuarioRegistra { get; set; } = string.Empty;
    }

    public class RegistroFallecimientoResponseDTO
    {
        public string Codigo { get; set; } = string.Empty;
        public string CodigoProcedimiento { get; set; } = string.Empty;
        public string NombrePaciente { get; set; } = string.Empty;
        public string CodigoPaciente { get; set; } = string.Empty;
        public DateTime FechaHoraFallecimiento { get; set; }
        public string CausaFallecimiento { get; set; } = string.Empty;
        public string UsuarioRegistra { get; set; } = string.Empty;
        public string? NumeroActaLegal { get; set; }
        public string EstadoLegal { get; set; } = string.Empty;
    }

    /// <summary>
    /// Payload que ENVÍA Gestión Legal a Gestión Quirúrgica mediante POST
    /// para adjuntar el número de acta y confirmar la legalización.
    /// Ruta: POST /api/RegistrosFallecimiento/{codigo}/legalizacion
    /// </summary>
    public class LegalizacionDTO
    {
        /// <summary>Número de acta generado por Gestión Legal</summary>
        public string NumeroActaLegal { get; set; } = string.Empty;

        /// <summary>"Legalizado" o "Rechazado"</summary>
        public string EstadoLegal { get; set; } = string.Empty;

        /// <summary>Observaciones adicionales de Gestión Legal</summary>
        public string? Observaciones { get; set; }
    }

    // ── Programación de Cirugía (POST para médicos) ───────────────────────────

    public class ProgramacionCirugiaRequestDTO
    {
        public string Codigo { get; set; } = string.Empty;
        public string CodigoPaciente { get; set; } = string.Empty;
        public string CodigoSala { get; set; } = string.Empty;
        public string CodigoCirujanoSolicitante { get; set; } = string.Empty;
        public DateTime HoraInicioEstimada { get; set; }
        public DateTime HoraFinEstimada { get; set; }
        public string TipoCirugia { get; set; } = string.Empty;

        /// <summary>Alta / Media / Baja</summary>
        public string Prioridad { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
    }

    public class ProgramacionCirugiaResponseDTO
    {
        public string Codigo { get; set; } = string.Empty;
        public string CodigoPaciente { get; set; } = string.Empty;
        public string NombrePaciente { get; set; } = string.Empty;
        public string CodigoSala { get; set; } = string.Empty;
        public string NumeroSala { get; set; } = string.Empty;
        public string CodigoCirujano { get; set; } = string.Empty;
        public string NombreCirujano { get; set; } = string.Empty;
        public DateTime FechaSolicitada { get; set; }
        public DateTime HoraInicioEstimada { get; set; }
        public DateTime HoraFinEstimada { get; set; }
        public string TipoCirugia { get; set; } = string.Empty;
        public string Prioridad { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
        public string EstadoProgramacion { get; set; } = string.Empty;
    }

    /// <summary>
    /// Payload para cambiar el estado de la programación
    /// (ej: Jefatura aprueba o rechaza)
    /// </summary>
    public class CambioEstadoProgramacionDTO
    {
        /// <summary>"Aprobada" | "Rechazada"</summary>
        public string NuevoEstado { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
    }
}
