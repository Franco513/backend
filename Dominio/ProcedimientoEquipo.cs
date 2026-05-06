namespace Gestion_Quirurgica.Dominio
{
    
    public class ProcedimientoEquipo
    {
        public int Id { get; set; }

        public string Codigo { get; set; } = string.Empty;

        public int ProcedimientoId { get; set; }
        public int EquipoId { get; set; }

        public string EstadoUso { get; set; } = string.Empty; 
        public string Observaciones { get; set; } = string.Empty;

        public string Estado { get; set; } = "Activo";

        public ProcedimientoQuirurgico Procedimiento { get; set; } = null!;
        public EquipoMedico Equipo { get; set; } = null!;
    }
}
