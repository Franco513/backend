namespace Gestion_Quirurgica.Dominio
{
    
    public class Paciente
    {
        public int Id { get; set; }

        public string Codigo { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;
        public int Edad { get; set; }
        public string DiagnosticoPre { get; set; } = string.Empty;
        public string Alergias { get; set; } = string.Empty;
        public string GrupoSanguineo { get; set; } = string.Empty;

        public string Estado { get; set; } = "Activo";

        public ICollection<ProcedimientoQuirurgico> Procedimientos { get; set; }
            = new List<ProcedimientoQuirurgico>();
    }
}
