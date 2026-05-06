using Gestion_Quirurgica.Data;
using Gestion_Quirurgica.Dominio;
using Gestion_Quirurgica.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gestion_Quirurgica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalasQuirurgicasController : ControllerBase
    {
        private readonly GestionQuirurgicaContext _context;
        public SalasQuirurgicasController(GestionQuirurgicaContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SalaQuirurgicaResponseDTO>>> GetAll()
        {
            var salas = await (
                from s in _context.SalasQuirurgicas
                where s.Estado == "Activo"
                select new SalaQuirurgicaResponseDTO
                {
                    Codigo           = s.Codigo,
                    NumeroSala       = s.NumeroSala,
                    NivelEsterilidad = s.NivelEsterilidad,
                    Disponible       = s.Disponible,
                    UltimaLimpieza   = s.UltimaLimpieza
                }
            ).ToListAsync();

            return Ok(salas);
        }

        [HttpGet("{codigo}")]
        public async Task<ActionResult<SalaQuirurgicaResponseDTO>> GetByCodigo(string codigo)
        {
            var sala = await (
                from s in _context.SalasQuirurgicas
                where s.Codigo == codigo && s.Estado == "Activo"
                select new SalaQuirurgicaResponseDTO
                {
                    Codigo           = s.Codigo,
                    NumeroSala       = s.NumeroSala,
                    NivelEsterilidad = s.NivelEsterilidad,
                    Disponible       = s.Disponible,
                    UltimaLimpieza   = s.UltimaLimpieza
                }
            ).FirstOrDefaultAsync();

            if (sala is null)
                return NotFound(new { mensaje = $"Sala '{codigo}' no encontrada." });

            return Ok(sala);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromQuery] SalaQuirurgicaRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Codigo) || string.IsNullOrWhiteSpace(dto.NumeroSala))
                return BadRequest(new { mensaje = "Los campos 'codigo' y 'numeroSala' son obligatorios." });

            if (await _context.SalasQuirurgicas.AnyAsync(s => s.Codigo == dto.Codigo))
                return Conflict(new { mensaje = $"Ya existe una sala con código '{dto.Codigo}'." });

            if (await _context.SalasQuirurgicas.AnyAsync(s => s.NumeroSala == dto.NumeroSala))
                return Conflict(new { mensaje = $"Ya existe una sala con número '{dto.NumeroSala}'." });

            _context.SalasQuirurgicas.Add(new SalaQuirurgica
            {
                Codigo           = dto.Codigo,
                NumeroSala       = dto.NumeroSala,
                NivelEsterilidad = dto.NivelEsterilidad,
                Disponible       = dto.Disponible,
                UltimaLimpieza   = dto.UltimaLimpieza,
                Estado           = "Activo"
            });

            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetByCodigo), new { codigo = dto.Codigo },
                new { mensaje = "Sala quirúrgica creada correctamente.", codigo = dto.Codigo });
        }

        [HttpPut("{codigo}")]
        public async Task<ActionResult> Update(string codigo, [FromQuery] SalaQuirurgicaRequestDTO dto)
        {
            var sala = await _context.SalasQuirurgicas
                .FirstOrDefaultAsync(s => s.Codigo == codigo && s.Estado == "Activo");

            if (sala is null)
                return NotFound(new { mensaje = $"Sala '{codigo}' no encontrada." });

            sala.NivelEsterilidad = dto.NivelEsterilidad;
            sala.Disponible       = dto.Disponible;
            sala.UltimaLimpieza   = dto.UltimaLimpieza;

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Sala quirúrgica actualizada correctamente." });
        }

        [HttpDelete("{codigo}")]
        public async Task<ActionResult> Delete(string codigo)
        {
            var sala = await _context.SalasQuirurgicas
                .FirstOrDefaultAsync(s => s.Codigo == codigo && s.Estado == "Activo");

            if (sala is null)
                return NotFound(new { mensaje = $"Sala '{codigo}' no encontrada." });

            sala.Estado = "Inactivo";
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = $"Sala '{codigo}' marcada como Inactiva (Soft Delete)." });
        }
    }
}
