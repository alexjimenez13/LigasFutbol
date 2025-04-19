using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LigasFutbol.Data;
using LigasFutbol.Models;

namespace LigasFutbol.Controllers
{
    public class LigaController : Controller
    {
        private readonly AppDbContext _db;
        public LigaController(AppDbContext db) => _db = db;

        public IActionResult Index() => View();

        [HttpGet]
        public async Task<JsonResult> Listar()
        {
            var lista = await _db.FUT_LIGAS
                .Select(l => new {
                    ligaId = l.LigaId,
                    nombre = l.Nombre,
                    descripcion = l.Descripcion,
                    fechaInicio = l.FechaInicio.ToString("yyyy-MM-dd"),
                    fechaFin = l.FechaFin.HasValue ? l.FechaFin.Value.ToString("yyyy-MM-dd") : string.Empty,
                    estado = l.Estado
                })
                .ToListAsync();
            return Json(new { data = lista });
        }

        [HttpGet]
        public async Task<JsonResult> Obtener(int id)
        {
            var l = await _db.FUT_LIGAS.FindAsync(id);
            if (l == null) return Json(null);
            return Json(new
            {
                ligaId = l.LigaId,
                nombre = l.Nombre,
                descripcion = l.Descripcion,
                fechaInicio = l.FechaInicio.ToString("yyyy-MM-dd"),
                fechaFin = l.FechaFin?.ToString("yyyy-MM-dd"),
                estado = l.Estado
            });
        }

        [HttpPost]
        public async Task<JsonResult> Guardar([FromBody] Liga model)
        {
            bool resultado = true;
            try
            {
                if (model.LigaId == 0)
                    _db.FUT_LIGAS.Add(model);
                else
                    _db.FUT_LIGAS.Update(model);
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
                var l = await _db.FUT_LIGAS.FindAsync(id);
                if (l != null)
                {
                    _db.FUT_LIGAS.Remove(l);
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
