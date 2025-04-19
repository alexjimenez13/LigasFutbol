using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LigasFutbol.Data;
using LigasFutbol.Models;

namespace LigasFutbol.Controllers
{
    public class ResultadoController : Controller
    {
        private readonly AppDbContext _db;
        public ResultadoController(AppDbContext db) => _db = db;

        public IActionResult Index() => View();

        [HttpGet]
        public async Task<JsonResult> Listar()
        {
            var lista = await (from r in _db.FUT_RESULTADOS
                               join p in _db.FUT_PARTIDOS on r.PartidoId equals p.PartidoId
                               join el in _db.FUT_EQUIPOS on p.EquipoLocalId equals el.EquipoId
                               join ev in _db.FUT_EQUIPOS on p.EquipoVisitanteId equals ev.EquipoId
                               select new
                               {
                                   resultadoId = r.ResultadoId,
                                   partido = el.Nombre + " vs " + ev.Nombre,
                                   golesLocal = r.GolesLocal,
                                   golesVisitante = r.GolesVisita,
                                   resultadoTexto = r.GolesLocal > r.GolesVisita
                                                      ? el.Nombre
                                                      : r.GolesLocal < r.GolesVisita
                                                        ? ev.Nombre
                                                        : "Empate"
                               })
                              .ToListAsync();

            return Json(new { data = lista });
        }

        [HttpGet]
        public async Task<JsonResult> Obtener(int id)
        {
            var r = await _db.FUT_RESULTADOS.FindAsync(id);
            if (r == null) return Json(null);

            return Json(new
            {
                resultadoId = r.ResultadoId,
                partidoId = r.PartidoId,
                golesLocal = r.GolesLocal,
                golesVisitante = r.GolesVisita
            });
        }

        [HttpGet]
        public async Task<JsonResult> GetPartidos()
        {
            var partidos = await (from p in _db.FUT_PARTIDOS
                                  join el in _db.FUT_EQUIPOS on p.EquipoLocalId equals el.EquipoId
                                  join ev in _db.FUT_EQUIPOS on p.EquipoVisitanteId equals ev.EquipoId
                                  join r in _db.FUT_RESULTADOS on p.PartidoId equals r.PartidoId into pr
                                  from r2 in pr.DefaultIfEmpty()
                                  select new
                                  {
                                      p.PartidoId,
                                      descripcion = el.Nombre + " vs " + ev.Nombre,
                                      tieneResultado = r2 != null
                                  })
                                 .ToListAsync();

            return Json(partidos);
        }

        [HttpPost]
        public async Task<JsonResult> Guardar([FromBody] Resultado model)
        {
            bool ok = true;
            try
            {
                if (model.ResultadoId == 0) _db.FUT_RESULTADOS.Add(model);
                else _db.FUT_RESULTADOS.Update(model);

                await _db.SaveChangesAsync();
            }
            catch
            {
                ok = false;
            }
            return Json(new { resultado = ok });
        }

        [HttpGet]
        public async Task<JsonResult> Eliminar(int id)
        {
            bool ok = true;
            try
            {
                var r = await _db.FUT_RESULTADOS.FindAsync(id);
                if (r != null)
                {
                    _db.FUT_RESULTADOS.Remove(r);
                    await _db.SaveChangesAsync();
                }
            }
            catch
            {
                ok = false;
            }
            return Json(new { resultado = ok });
        }
    }
}
