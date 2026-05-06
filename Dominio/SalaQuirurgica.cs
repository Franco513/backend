namespace Gestion_Quirurgica.Dominio
{
    
    public class SalaQuirurgica
    {
        public int Id { get; set; }

        public string Codigo { get; set; } = string.Empty;

        public string NumeroSala { get; set; } = string.Empty;
        public string NivelEsterilidad { get; set; } = string.Empty;
        public bool Disponible { get; set; } = true;
        public DateTime UltimaLimpieza { get; set; }

        public string Estado { get; set; } = "Activo";

        public ICollection<ProcedimientoQuirurgico> Procedimientos { get; set; }
            = new List<ProcedimientoQuirurgico>();
    }
}
