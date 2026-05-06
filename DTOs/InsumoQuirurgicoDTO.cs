namespace Gestion_Quirurgica.DTOs
{
    // ── Insumos Quirúrgicos (para Logística) ─────────────────────────────────

    public class InsumoQuirurgicoRequestDTO
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string UnidadMedida { get; set; } = string.Empty;
        public int CantidadMinima { get; set; }
        public int CantidadActual { get; set; }
    }

    /// <summary>
    /// Respuesta que recibe Logística cuando llama GET /api/InventarioInsumos
    /// Incluye cuánto hay, cuánto se necesita y la diferencia.
    /// </summary>
    public class InsumoQuirurgicoResponseDTO
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string UnidadMedida { get; set; } = string.Empty;

        /// <summary>Stock mínimo requerido para operar</summary>
        public int CantidadMinima { get; set; }

        /// <summary>Stock actual en el área quirúrgica</summary>
        public int CantidadActual { get; set; }

        /// <summary>
        /// Cuánto le falta. Si es 0 = OK. Si es > 0 = necesita reposición.
        /// Logística usa este campo para planificar el reabastecimiento.
        /// </summary>
        public int CantidadNecesaria { get; set; }

        /// <summary>"OK" si no falta nada, "Reponer" si hay déficit</summary>
        public string Alerta { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para que Logística envíe la lista de insumos que necesita verificar.
    /// GET /api/InventarioInsumos/verificar recibe este payload como query o body.
    /// </summary>
    public class VerificacionInsumoDTO
    {
        public string CodigoInsumo { get; set; } = string.Empty;
        /// <summary>Cantidad mínima que Logística dice que debería haber</summary>
        public int CantidadEsperada { get; set; }
    }

    public class ResultadoVerificacionDTO
    {
        public string CodigoInsumo { get; set; } = string.Empty;
        public string NombreInsumo { get; set; } = string.Empty;
        public string UnidadMedida { get; set; } = string.Empty;
        public int CantidadEsperada { get; set; }
        public int CantidadActual { get; set; }
        public int CantidadFaltante { get; set; }
        public string Estado { get; set; } = string.Empty; // "OK" | "Déficit"
    }

    // ── Registro de Consumo de Insumos ────────────────────────────────────────

    public class ConsumoInsumoDTO
    {
        public string CodigoInsumo { get; set; } = string.Empty;
        /// <summary>Cantidad a descontar del stock actual</summary>
        public int CantidadConsumida { get; set; }
        public string UsuarioRegistra { get; set; } = string.Empty;
        /// <summary>Procedimiento que generó el consumo (opcional)</summary>
        public string? CodigoProcedimiento { get; set; }
    }
}
