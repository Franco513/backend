namespace Gestion_Quirurgica.Dominio
{
    
    public class RolQuirurgico
    {
        public int Id { get; set; }

        public string Codigo { get; set; } = string.Empty;

        public int ProcedimientoId { get; set; }
        public int CirujanoId { get; set; }

        public string Rol { get; set; } = string.Empty; 

        public string Estado { get; set; } = "Activo";

        public ProcedimientoQuirurgico Procedimiento { get; set; } = null!;
        public Cirujano Cirujano { get; set; } = null!;
    }
}
