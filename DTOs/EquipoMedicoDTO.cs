namespace Gestion_Quirurgica.DTOs
{
    public class EquipoMedicoRequestDTO
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Fabricante { get; set; } = string.Empty;
        public DateTime UltimoMantenimiento { get; set; }
        public bool EstadoOperativo { get; set; }
    }

    public class EquipoMedicoResponseDTO
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Fabricante { get; set; } = string.Empty;
        public DateTime UltimoMantenimiento { get; set; }
        public bool EstadoOperativo { get; set; }
    }
}
