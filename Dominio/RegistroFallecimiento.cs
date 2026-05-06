namespace Gestion_Quirurgica.Dominio
{
    /// <summary>
    /// Registra el fallecimiento de un paciente durante o después de un procedimiento quirúrgico.
    /// Gestión Legal recibe el reporte de este registro y adjunta su número de legalización.
    /// </summary>
    public class RegistroFallecimiento
    {
        public int Id { get; set; }

        public string Codigo { get; set; } = string.Empty;

        /// <summary>Referencia al procedimiento quirúrgico en curso</summary>
        public int ProcedimientoId { get; set; }

        /// <summary>Fecha y hora exacta del fallecimiento</summary>
        public DateTime FechaHoraFallecimiento { get; set; }

        /// <summary>Causa del fallecimiento según el médico</summary>
        public string CausaFallecimiento { get; set; } = string.Empty;

        /// <summary>Médico o enfermero que registra el evento</summary>
        public string UsuarioRegistra { get; set; } = string.Empty;

        /// <summary>
        /// Número de acta/resolución que devuelve Gestión Legal tras la legalización.
        /// Se rellena cuando Gestión Legal confirma el trámite.
        /// </summary>
        public string? NumeroActaLegal { get; set; }

        /// <summary>
        /// Estado del trámite legal:
        /// "Pendiente"  → enviado a Gestión Legal, esperando respuesta
        /// "Legalizado" → Gestión Legal ya emitió el acta
        /// "Rechazado"  → Gestión Legal requiere correcciones
        /// </summary>
        public string EstadoLegal { get; set; } = "Pendiente";

        public string Estado { get; set; } = "Activo";

        // Navegación
        public ProcedimientoQuirurgico Procedimiento { get; set; } = null!;
    }
}
