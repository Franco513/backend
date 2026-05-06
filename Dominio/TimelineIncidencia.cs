namespace Gestion_Quirurgica.Dominio
{
    
    public class TimelineIncidencia
    {
        public int Id { get; set; }

        public string Codigo { get; set; } = string.Empty;

        public int ProcedimientoId { get; set; }

        public DateTime Timestamp { get; set; }
        public string Tipo { get; set; } = string.Empty;       
        public string Descripcion { get; set; } = string.Empty;
        public string UsuarioRegistra { get; set; } = string.Empty;

        public string Estado { get; set; } = "Activo";

        public ProcedimientoQuirurgico Procedimiento { get; set; } = null!;
    }
}
