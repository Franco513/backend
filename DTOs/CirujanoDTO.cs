namespace Gestion_Quirurgica.DTOs
{
    public class CirujanoRequestDTO
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Especialidad { get; set; } = string.Empty;
        public int AnosExperiencia { get; set; }
    }

    public class CirujanoResponseDTO
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Especialidad { get; set; } = string.Empty;
        public int AnosExperiencia { get; set; }
    }
}
