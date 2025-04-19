using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LigasFutbol.Data;
using LigasFutbol.Models;

namespace LigasFutbol.Controllers
{
    public class EquipoController : Controller
    {
        private readonly AppDbContext _db;
        public EquipoController(AppDbContext db) => _db = db;

        public IActionResult Index() => View();

        [HttpGet]
        public async Task<JsonResult> Listar()
        {
            var lista = await (from e in _db.FUT_EQUIPOS
                               join l in _db.FUT_LIGAS on e.LigaId equals l.LigaId
                               select new
                               {
                                   equipoId = e.EquipoId,
                                   nombre = e.Nombre,
                                   ligaId = e.LigaId,
                                   ligaNombre = l.Nombre,
                                   entrenador = e.Entrenador,
                                   estado = e.Estado
                               })
                              .ToListAsync();

            return Json(new { data = lista });
        }

        [HttpGet]
        public async Task<JsonResult> Obtener(int id)
        {
            var e = await _db.FUT_EQUIPOS.FindAsync(id);
            if (e == null) return Json(null);

            return Json(new
            {
                equipoId = e.EquipoId,
                nombre = e.Nombre,
                ligaId = e.LigaId,
                entrenador = e.Entrenador,
                estado = e.Estado
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

        [HttpPost]
        public async Task<JsonResult> Guardar([FromBody] Equipo model)
        {
            bool resultado = true;
            try
            {
                if (model.EquipoId == 0)
                    _db.FUT_EQUIPOS.Add(model);
                else
                    _db.FUT_EQUIPOS.Update(model);

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
                var e = await _db.FUT_EQUIPOS.FindAsync(id);
                if (e != null)
                {
                    _db.FUT_EQUIPOS.Remove(e);
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
