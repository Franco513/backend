namespace Gestion_Quirurgica.Dominio
{
    
    public class ProcedimientoQuirurgico
    {
        public int Id { get; set; }

        public string Codigo { get; set; } = string.Empty;

        public int PacienteId { get; set; }
        public int SalaId { get; set; }

        public DateTime HoraInicio { get; set; }
        public DateTime HoraFin { get; set; }
        public string EstadoProcedimiento { get; set; } = string.Empty;
        public string Prioridad { get; set; } = string.Empty;

        public string Estado { get; set; } = "Activo";

        public Paciente Paciente { get; set; } = null!;
        public SalaQuirurgica Sala { get; set; } = null!;

        public ICollection<RolQuirurgico> Roles { get; set; }
            = new List<RolQuirurgico>();
        public ICollection<ProcedimientoEquipo> Equipos { get; set; }
            = new List<ProcedimientoEquipo>();
        public ICollection<TimelineIncidencia> Timeline { get; set; }
            = new List<TimelineIncidencia>();
    }
}
