using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LigasFutbol.Data;
using LigasFutbol.Models;

namespace LigasFutbol.Controllers
{
    public class JugadorController : Controller
    {
        private readonly AppDbContext _db;
        public JugadorController(AppDbContext db) => _db = db;

        public IActionResult Index() => View();

        [HttpGet]
        public async Task<JsonResult> Listar()
        {
            var lista = await (from j in _db.FUT_JUGADORES
                               join p in _db.FUT_POSICIONES on j.PosicionId equals p.PosicionId into pj
                               from p in pj.DefaultIfEmpty()
                               join e in _db.FUT_EQUIPOS on j.EquipoId equals e.EquipoId into ej
                               from e in ej.DefaultIfEmpty()
                               select new
                               {
                                   jugadorId = j.JugadorId,
                                   nombre = j.Nombre,
                                   apellido = j.Apellido,
                                   fechaNacimiento = j.FechaNacimiento.HasValue
                                                       ? j.FechaNacimiento.Value.ToString("yyyy-MM-dd")
                                                       : string.Empty,
                                   posicionId = j.PosicionId,
                                   posicionNombre = p != null ? p.Nombre : string.Empty,
                                   equipoId = j.EquipoId,
                                   equipoNombre = e != null ? e.Nombre : string.Empty
                               })
                              .ToListAsync();

            return Json(new { data = lista });
        }

        [HttpGet]
        public async Task<JsonResult> Obtener(int id)
        {
            var j = await _db.FUT_JUGADORES.FindAsync(id);
            if (j == null) return Json(null);

            return Json(new
            {
                jugadorId = j.JugadorId,
                nombre = j.Nombre,
                apellido = j.Apellido,
                fechaNacimiento = j.FechaNacimiento?.ToString("yyyy-MM-dd"),
                posicionId = j.PosicionId,
                equipoId = j.EquipoId
            });
        }

        [HttpGet]
        public async Task<JsonResult> GetPosiciones()
        {
            var lista = await _db.FUT_POSICIONES
                                 .Where(p => p.Estado)
                                 .Select(p => new { p.PosicionId, p.Nombre })
                                 .ToListAsync();
            return Json(lista);
        }

        [HttpGet]
        public async Task<JsonResult> GetEquipos()
        {
            var lista = await _db.FUT_EQUIPOS
                                 .Where(e => e.Estado)
                                 .Select(e => new { e.EquipoId, e.Nombre })
                                 .ToListAsync();
            return Json(lista);
        }

        [HttpPost]
        public async Task<JsonResult> Guardar([FromBody] Jugador model)
        {
            bool resultado = true;
            try
            {
                if (model.JugadorId == 0)
                    _db.FUT_JUGADORES.Add(model);
                else
                    _db.FUT_JUGADORES.Update(model);

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
                var j = await _db.FUT_JUGADORES.FindAsync(id);
                if (j != null)
                {
                    _db.FUT_JUGADORES.Remove(j);
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
