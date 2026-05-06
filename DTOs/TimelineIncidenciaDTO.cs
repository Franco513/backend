namespace Gestion_Quirurgica.DTOs
{
    public class TimelineIncidenciaRequestDTO
    {
        public string Codigo { get; set; } = string.Empty;
        public string CodigoProcedimiento { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string UsuarioRegistra { get; set; } = string.Empty;
    }

    public class TimelineIncidenciaResponseDTO
    {
        public string Codigo { get; set; } = string.Empty;
        public string CodigoProcedimiento { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string UsuarioRegistra { get; set; } = string.Empty;
    }
}
