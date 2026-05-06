namespace Gestion_Quirurgica.DTOs
{
    public class ProcedimientoQuirurgicoRequestDTO
    {
        public string Codigo { get; set; } = string.Empty;
        public string CodigoPaciente { get; set; } = string.Empty;
        public string CodigoSala { get; set; } = string.Empty;
        public DateTime HoraInicio { get; set; }
        public DateTime HoraFin { get; set; }
        public string EstadoProcedimiento { get; set; } = string.Empty;
        public string Prioridad { get; set; } = string.Empty;
    }

    public class ProcedimientoQuirurgicoResponseDTO
    {
        public string Codigo { get; set; } = string.Empty;
        public string CodigoPaciente { get; set; } = string.Empty;
        public string NombrePaciente { get; set; } = string.Empty;
        public string CodigoSala { get; set; } = string.Empty;
        public string NumeroSala { get; set; } = string.Empty;
        public DateTime HoraInicio { get; set; }
        public DateTime HoraFin { get; set; }
        public string EstadoProcedimiento { get; set; } = string.Empty;
        public string Prioridad { get; set; } = string.Empty;
    }
}
