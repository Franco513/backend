namespace Gestion_Quirurgica.DTOs
{
    public class RolQuirurgicoRequestDTO
    {
        public string Codigo { get; set; } = string.Empty;
        public string CodigoProcedimiento { get; set; } = string.Empty;
        public string CodigoCirujano { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
    }

    public class RolQuirurgicoResponseDTO
    {
        public string Codigo { get; set; } = string.Empty;
        public string CodigoProcedimiento { get; set; } = string.Empty;
        public string CodigoCirujano { get; set; } = string.Empty;
        public string NombreCirujano { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
    }
}
