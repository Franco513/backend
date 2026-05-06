using Gestion_Quirurgica.Data;
using Gestion_Quirurgica.Dominio;
using Gestion_Quirurgica.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gestion_Quirurgica.Controllers
{
    /// <summary>
    /// Controlador de Inventario de Insumos Quirúrgicos.
    /// 
    /// INTEGRACIÓN CON LOGÍSTICA:
    ///   - GET /api/InventarioInsumos  → Logística llama este endpoint todas las noches
    ///     para ver el estado actual del inventario y determinar qué reabastecer.
    ///   - GET /api/InventarioInsumos/verificar  → Logística envía la lista de insumos
    ///     con sus cantidades mínimas esperadas y recibe un reporte de déficit.
    ///   - POST /api/InventarioInsumos/consumo   → Gestión Quirúrgica registra el consumo
    ///     para mantener el stock actualizado.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class InventarioInsumosController : ControllerBase
    {
        private readonly GestionQuirurgicaContext _context;
        public InventarioInsumosController(GestionQuirurgicaContext context) => _context = context;

        // ─────────────────────────────────────────────────────────────────────
        // GET /api/InventarioInsumos
        // Logística llama este endpoint para ver el estado completo del inventario.
        // Responde con la lista de insumos, stock actual vs mínimo, y alertas.
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InsumoQuirurgicoResponseDTO>>> GetAll()
        {
            var insumos = await _context.InsumosQuirurgicos
                .Where(i => i.Estado == "Activo")
                .OrderBy(i => i.Nombre)
                .ToListAsync();

            var resultado = insumos.Select(i => new InsumoQuirurgicoResponseDTO
            {
                Codigo            = i.Codigo,
                Nombre            = i.Nombre,
                UnidadMedida      = i.UnidadMedida,
                CantidadMinima    = i.CantidadMinima,
                CantidadActual    = i.CantidadActual,
                CantidadNecesaria = i.CantidadNecesaria,
                Alerta            = i.CantidadNecesaria > 0 ? "Reponer" : "OK"
            }).ToList();

            return Ok(resultado);
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET /api/InventarioInsumos/{codigo}
        // Consulta un insumo específico.
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet("{codigo}")]
        public async Task<ActionResult<InsumoQuirurgicoResponseDTO>> GetByCodigo(string codigo)
        {
            var i = await _context.InsumosQuirurgicos
                .FirstOrDefaultAsync(x => x.Codigo == codigo && x.Estado == "Activo");

            if (i is null)
                return NotFound(new { mensaje = $"Insumo '{codigo}' no encontrado." });

            return Ok(new InsumoQuirurgicoResponseDTO
            {
                Codigo            = i.Codigo,
                Nombre            = i.Nombre,
                UnidadMedida      = i.UnidadMedida,
                CantidadMinima    = i.CantidadMinima,
                CantidadActual    = i.CantidadActual,
                CantidadNecesaria = i.CantidadNecesaria,
                Alerta            = i.CantidadNecesaria > 0 ? "Reponer" : "OK"
            });
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET /api/InventarioInsumos/verificar
        // ENDPOINT PRINCIPAL PARA LOGÍSTICA.
        // Logística manda la lista de insumos con la cantidad mínima que ellos
        // consideran necesaria. Gestión Quirúrgica responde diciendo cuánto falta
        // de cada insumo comparando con el stock actual.
        //
        // Ejemplo de query string:
        //   ?insumos[0].CodigoInsumo=INS-001&insumos[0].CantidadEsperada=10
        //   &insumos[1].CodigoInsumo=INS-002&insumos[1].CantidadEsperada=2
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet("verificar")]
        public async Task<ActionResult<IEnumerable<ResultadoVerificacionDTO>>> Verificar(
            [FromQuery] List<VerificacionInsumoDTO> insumos)
        {
            if (insumos is null || insumos.Count == 0)
                return BadRequest(new { mensaje = "Debes enviar al menos un insumo para verificar." });

            var codigos = insumos.Select(x => x.CodigoInsumo).ToList();
            var stock = await _context.InsumosQuirurgicos
                .Where(i => codigos.Contains(i.Codigo) && i.Estado == "Activo")
                .ToDictionaryAsync(i => i.Codigo);

            var resultado = new List<ResultadoVerificacionDTO>();

            foreach (var item in insumos)
            {
                if (!stock.TryGetValue(item.CodigoInsumo, out var insumo))
                {
                    resultado.Add(new ResultadoVerificacionDTO
                    {
                        CodigoInsumo    = item.CodigoInsumo,
                        NombreInsumo    = "No encontrado",
                        CantidadEsperada = item.CantidadEsperada,
                        CantidadActual  = 0,
                        CantidadFaltante = item.CantidadEsperada,
                        Estado          = "Déficit"
                    });
                    continue;
                }

                int faltante = Math.Max(0, item.CantidadEsperada - insumo.CantidadActual);
                resultado.Add(new ResultadoVerificacionDTO
                {
                    CodigoInsumo     = insumo.Codigo,
                    NombreInsumo     = insumo.Nombre,
                    UnidadMedida     = insumo.UnidadMedida,
                    CantidadEsperada = item.CantidadEsperada,
                    CantidadActual   = insumo.CantidadActual,
                    CantidadFaltante = faltante,
                    Estado           = faltante > 0 ? "Déficit" : "OK"
                });
            }

            return Ok(resultado);
        }

        // ─────────────────────────────────────────────────────────────────────
        // POST /api/InventarioInsumos
        // Registra un nuevo insumo en el inventario.
        // ─────────────────────────────────────────────────────────────────────
        [HttpPost]
        public async Task<ActionResult> Create([FromQuery] InsumoQuirurgicoRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Codigo))
                return BadRequest(new { mensaje = "El campo 'codigo' es obligatorio." });

            if (await _context.InsumosQuirurgicos.AnyAsync(i => i.Codigo == dto.Codigo))
                return Conflict(new { mensaje = $"Ya existe un insumo con código '{dto.Codigo}'." });

            _context.InsumosQuirurgicos.Add(new InsumoQuirurgico
            {
                Codigo         = dto.Codigo,
                Nombre         = dto.Nombre,
                UnidadMedida   = dto.UnidadMedida,
                CantidadMinima = dto.CantidadMinima,
                CantidadActual = dto.CantidadActual,
                Estado         = "Activo"
            });

            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetByCodigo), new { codigo = dto.Codigo },
                new { mensaje = "Insumo registrado correctamente.", codigo = dto.Codigo });
        }

        // ─────────────────────────────────────────────────────────────────────
        // POST /api/InventarioInsumos/consumo
        // Gestión Quirúrgica registra el consumo de un insumo durante una cirugía.
        // Descuenta la cantidad del stock actual.
        // ─────────────────────────────────────────────────────────────────────
        [HttpPost("consumo")]
        public async Task<ActionResult> RegistrarConsumo([FromQuery] ConsumoInsumoDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.CodigoInsumo))
                return BadRequest(new { mensaje = "El campo 'CodigoInsumo' es obligatorio." });

            var insumo = await _context.InsumosQuirurgicos
                .FirstOrDefaultAsync(i => i.Codigo == dto.CodigoInsumo && i.Estado == "Activo");

            if (insumo is null)
                return NotFound(new { mensaje = $"Insumo '{dto.CodigoInsumo}' no encontrado." });

            if (dto.CantidadConsumida <= 0)
                return BadRequest(new { mensaje = "La cantidad consumida debe ser mayor a 0." });

            if (dto.CantidadConsumida > insumo.CantidadActual)
                return BadRequest(new
                {
                    mensaje = $"Stock insuficiente. Disponible: {insumo.CantidadActual}, solicitado: {dto.CantidadConsumida}."
                });

            insumo.CantidadActual -= dto.CantidadConsumida;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje          = "Consumo registrado correctamente.",
                codigoInsumo     = insumo.Codigo,
                nombreInsumo     = insumo.Nombre,
                cantidadDescontada = dto.CantidadConsumida,
                stockRestante    = insumo.CantidadActual,
                alerta           = insumo.CantidadNecesaria > 0
                    ? $"ATENCIÓN: Falta reponer {insumo.CantidadNecesaria} {insumo.UnidadMedida}"
                    : "Stock OK"
            });
        }

        // ─────────────────────────────────────────────────────────────────────
        // PUT /api/InventarioInsumos/{codigo}
        // Actualiza los datos de un insumo (cantidad mínima, etc.)
        // ─────────────────────────────────────────────────────────────────────
        [HttpPut("{codigo}")]
        public async Task<ActionResult> Update(string codigo, [FromQuery] InsumoQuirurgicoRequestDTO dto)
        {
            var insumo = await _context.InsumosQuirurgicos
                .FirstOrDefaultAsync(i => i.Codigo == codigo && i.Estado == "Activo");

            if (insumo is null)
                return NotFound(new { mensaje = $"Insumo '{codigo}' no encontrado." });

            insumo.Nombre         = dto.Nombre;
            insumo.UnidadMedida   = dto.UnidadMedida;
            insumo.CantidadMinima = dto.CantidadMinima;
            insumo.CantidadActual = dto.CantidadActual;

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Insumo actualizado correctamente." });
        }

        // ─────────────────────────────────────────────────────────────────────
        // DELETE /api/InventarioInsumos/{codigo} (Soft Delete)
        // ─────────────────────────────────────────────────────────────────────
        [HttpDelete("{codigo}")]
        public async Task<ActionResult> Delete(string codigo)
        {
            var insumo = await _context.InsumosQuirurgicos
                .FirstOrDefaultAsync(i => i.Codigo == codigo && i.Estado == "Activo");

            if (insumo is null)
                return NotFound(new { mensaje = $"Insumo '{codigo}' no encontrado." });

            insumo.Estado = "Inactivo";
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = $"Insumo '{codigo}' marcado como Inactivo." });
        }
    }
}
