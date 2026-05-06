namespace Gestion_Quirurgica.Dominio
{
    
    public class EquipoMedico
    {
        public int Id { get; set; }

        public string Codigo { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;
        public string Fabricante { get; set; } = string.Empty;
        public DateTime UltimoMantenimiento { get; set; }
        public bool EstadoOperativo { get; set; } = true;

        public string Estado { get; set; } = "Activo";

        public ICollection<ProcedimientoEquipo> ProcedimientosEquipo { get; set; }
            = new List<ProcedimientoEquipo>();
    }
}
