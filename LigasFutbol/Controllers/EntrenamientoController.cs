using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LigasFutbol.Data;
using LigasFutbol.Models;

namespace LigasFutbol.Controllers
{
    public class EntrenamientoController : Controller
    {
        private readonly AppDbContext _db;
        public EntrenamientoController(AppDbContext db) => _db = db;

        public IActionResult Index() => View();

        [HttpGet]
        public async Task<JsonResult> Listar()
        {
            var lista = await (from e in _db.FUT_ENTRENAMIENTOS
                               join eq in _db.FUT_EQUIPOS on e.EquipoId equals eq.EquipoId
                               select new
                               {
                                   entrenamientoId = e.EntrenamientoId,
                                   equipoId = e.EquipoId,
                                   equipo = eq.Nombre,
                                   fechaHora = e.FechaHora.ToString("yyyy-MM-ddTHH:mm"),
                                   ubicacion = e.Ubicacion,
                                   estado = e.Estado
                               })
                              .ToListAsync();
            return Json(new { data = lista });
        }

        [HttpGet]
        public async Task<JsonResult> Obtener(int id)
        {
            var e = await _db.FUT_ENTRENAMIENTOS.FindAsync(id);
            if (e == null) return Json(null);
            return Json(new
            {
                entrenamientoId = e.EntrenamientoId,
                equipoId = e.EquipoId,
                fechaHora = e.FechaHora.ToString("yyyy-MM-ddTHH:mm"),
                ubicacion = e.Ubicacion,
                estado = e.Estado
            });
        }

        [HttpGet]
        public async Task<JsonResult> GetEquipos()
        {
            var equipos = await _db.FUT_EQUIPOS
                                    .Where(eq => eq.Estado)
                                    .Select(eq => new { eq.EquipoId, eq.Nombre })
                                    .ToListAsync();
            return Json(equipos);
        }

        [HttpPost]
        public async Task<JsonResult> Guardar([FromBody] Entrenamiento model)
        {
            bool resultado = true;
            try
            {
                model.FechaHora = model.FechaHora;
                if (model.EntrenamientoId == 0)
                {
                    model.Estado = true;
                    _db.FUT_ENTRENAMIENTOS.Add(model);
                }
                else
                {
                    _db.FUT_ENTRENAMIENTOS.Update(model);
                }
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
                var e = await _db.FUT_ENTRENAMIENTOS.FindAsync(id);
                if (e != null)
                {
                    _db.FUT_ENTRENAMIENTOS.Remove(e);
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
