using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LigasFutbol.Data;
using LigasFutbol.Models;

namespace LigasFutbol.Controllers
{
    public class PartidoController : Controller
    {
        private readonly AppDbContext _db;
        public PartidoController(AppDbContext db) => _db = db;

        public IActionResult Index() => View();

        [HttpGet]
        public async Task<JsonResult> Listar()
        {
            var lista = await (from p in _db.FUT_PARTIDOS
                               join l in _db.FUT_LIGAS on p.LigaId equals l.LigaId
                               join el in _db.FUT_EQUIPOS on p.EquipoLocalId equals el.EquipoId
                               join ev in _db.FUT_EQUIPOS on p.EquipoVisitanteId equals ev.EquipoId
                               select new
                               {
                                   partidoId = p.PartidoId,
                                   ligaNombre = l.Nombre,
                                   equipoLocalNombre = el.Nombre,
                                   equipoVisitanteNombre = ev.Nombre,
                                   fechaHora = p.FechaHora.ToString("yyyy-MM-dd HH:mm"),
                                   estado = p.Estado
                               })
                              .ToListAsync();
            return Json(new { data = lista });
        }

        [HttpGet]
        public async Task<JsonResult> Obtener(int id)
        {
            var p = await _db.FUT_PARTIDOS.FindAsync(id);
            if (p == null) return Json(null);
            return Json(new
            {
                partidoId = p.PartidoId,
                ligaId = p.LigaId,
                equipoLocalId = p.EquipoLocalId,
                equipoVisitanteId = p.EquipoVisitanteId,
                fechaHora = p.FechaHora.ToString("yyyy-MM-ddTHH:mm"),
                estado = p.Estado
            });
        }

        [HttpGet]
        public async Task<JsonResult> GetLigas()
        {
            var ligas = await _db.FUT_LIGAS
                                 .Where(l => l.Estado)
                                 .Select(l => new { l.LigaId, l.Nombre })
                                 .ToListAsync();
            return Json(ligas);
        }

        [HttpGet]
        public async Task<JsonResult> GetEquipos()
        {
            var equipos = await _db.FUT_EQUIPOS
                                   .Where(e => e.Estado)
                                   .Select(e => new { e.EquipoId, e.Nombre, e.LigaId })
                                   .ToListAsync();
            return Json(equipos);
        }

        [HttpPost]
        public async Task<JsonResult> Guardar([FromBody] Partido model)
        {
            bool resultado = true;
            try
            {
                if (model.PartidoId == 0)
                    _db.FUT_PARTIDOS.Add(model);
                else
                    _db.FUT_PARTIDOS.Update(model);
                await _db.SaveChangesAsync();
            }
            catch
            {
                resultado = false;
            }
            return Json(new { resultado });
        }

        [HttpGet]
        public async Task<JsonResult> Eliminar(int id)
        {
            bool resultado = true;
            try
            {
                var p = await _db.FUT_PARTIDOS.FindAsync(id);
                if (p != null)
                {
                    _db.FUT_PARTIDOS.Remove(p);
                    await _db.SaveChangesAsync();
                }
            }
            catch
            {
                resultado = false;
            }
            return Json(new { resultado });
        }
    }
}
