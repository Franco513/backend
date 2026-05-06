namespace Gestion_Quirurgica.Dominio
{
    /// <summary>
    /// Representa un insumo que se maneja dentro del área quirúrgica.
    /// Logística consulta este inventario mediante GET para determinar qué reabastecer.
    /// </summary>
    public class InsumoQuirurgico
    {
        public int Id { get; set; }

        public string Codigo { get; set; } = string.Empty;

        /// <summary>Nombre del insumo (ej: "Vendas", "Paquetes de sangre", "Escalpelos")</summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>Unidad de medida (ej: "unidad", "paquete", "caja")</summary>
        public string UnidadMedida { get; set; } = string.Empty;

        /// <summary>Cantidad mínima requerida para operar normalmente (stock mínimo)</summary>
        public int CantidadMinima { get; set; }

        /// <summary>Cantidad actualmente disponible en el área quirúrgica</summary>
        public int CantidadActual { get; set; }

        /// <summary>
        /// Calculado: cuántas unidades se necesitan reponer.
        /// = Max(0, CantidadMinima - CantidadActual)
        /// Logística lo usa para saber qué traer del almacén.
        /// </summary>
        public int CantidadNecesaria => Math.Max(0, CantidadMinima - CantidadActual);

        public string Estado { get; set; } = "Activo";
    }
}
