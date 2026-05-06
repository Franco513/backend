namespace Gestion_Quirurgica.Dominio
{
    /// <summary>
    /// Representa la programación de una cirugía hecha por un médico ANTES
    /// de convertirse en un ProcedimientoQuirurgico formal.
    /// Endpoint POST /api/ProgramacionCirugia
    /// </summary>
    public class ProgramacionCirugia
    {
        public int Id { get; set; }

        public string Codigo { get; set; } = string.Empty;

        public int PacienteId { get; set; }

        public int SalaId { get; set; }

        /// <summary>Cirujano principal que solicita la programación</summary>
        public int CirujanoSolicitanteId { get; set; }

        public DateTime FechaSolicitada { get; set; }

        public DateTime HoraInicioEstimada { get; set; }

        public DateTime HoraFinEstimada { get; set; }

        /// <summary>Descripción breve del tipo de intervención</summary>
        public string TipoCirugia { get; set; } = string.Empty;

        /// <summary>Alta / Media / Baja</summary>
        public string Prioridad { get; set; } = string.Empty;

        /// <summary>Observaciones pre-quirúrgicas del médico</summary>
        public string Observaciones { get; set; } = string.Empty;

        /// <summary>
        /// "Solicitada"  → recién enviada por el médico
        /// "Aprobada"    → jefatura quirúrgica aprobó la cirugía
        /// "Rechazada"   → rechazada (requiere correcciones)
        /// "Ejecutada"   → ya se convirtió en ProcedimientoQuirurgico
        /// </summary>
        public string EstadoProgramacion { get; set; } = "Solicitada";

        public string Estado { get; set; } = "Activo";

        // Navegación
        public Paciente Paciente { get; set; } = null!;
        public SalaQuirurgica Sala { get; set; } = null!;
        public Cirujano CirujanoSolicitante { get; set; } = null!;
    }
}
