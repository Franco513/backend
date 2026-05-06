namespace Gestion_Quirurgica.Dominio
{
    
    public class Cirujano
    {
        public int Id { get; set; }

        public string Codigo { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;
        public string Especialidad { get; set; } = string.Empty;
        public int AnosExperiencia { get; set; }

        public string Estado { get; set; } = "Activo";

        public ICollection<RolQuirurgico> Roles { get; set; }
            = new List<RolQuirurgico>();
    }
}
