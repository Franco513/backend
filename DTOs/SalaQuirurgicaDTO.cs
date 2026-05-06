namespace Gestion_Quirurgica.DTOs
{
    public class SalaQuirurgicaRequestDTO
    {
        public string Codigo { get; set; } = string.Empty;
        public string NumeroSala { get; set; } = string.Empty;
        public string NivelEsterilidad { get; set; } = string.Empty;
        public bool Disponible { get; set; }
        public DateTime UltimaLimpieza { get; set; }
    }

    public class SalaQuirurgicaResponseDTO
    {
        public string Codigo { get; set; } = string.Empty;
        public string NumeroSala { get; set; } = string.Empty;
        public string NivelEsterilidad { get; set; } = string.Empty;
        public bool Disponible { get; set; }
        public DateTime UltimaLimpieza { get; set; }
    }
}
