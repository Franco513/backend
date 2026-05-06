namespace Gestion_Quirurgica.DTOs
{
    public class ProcedimientoEquipoRequestDTO
    {
        public string Codigo { get; set; } = string.Empty;
        public string CodigoProcedimiento { get; set; } = string.Empty;
        public string CodigoEquipo { get; set; } = string.Empty;
        public string EstadoUso { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
    }

    public class ProcedimientoEquipoResponseDTO
    {
        public string Codigo { get; set; } = string.Empty;
        public string CodigoProcedimiento { get; set; } = string.Empty;
        public string CodigoEquipo { get; set; } = string.Empty;
        public string NombreEquipo { get; set; } = string.Empty;
        public string EstadoUso { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
    }
}
