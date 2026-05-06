namespace Gestion_Quirurgica.DTOs
{
    public class PacienteRequestDTO
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public int Edad { get; set; }
        public string DiagnosticoPre { get; set; } = string.Empty;
        public string Alergias { get; set; } = string.Empty;
        public string GrupoSanguineo { get; set; } = string.Empty;
    }

    public class PacienteResponseDTO
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public int Edad { get; set; }
        public string DiagnosticoPre { get; set; } = string.Empty;
        public string Alergias { get; set; } = string.Empty;
        public string GrupoSanguineo { get; set; } = string.Empty;
    }
}
