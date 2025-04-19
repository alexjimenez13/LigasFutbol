using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LigasFutbol.Data;
using LigasFutbol.Models;

namespace LigasFutbol.Controllers
{
    public class EstadisticaController : Controller
    {
        private readonly AppDbContext _db;
        public EstadisticaController(AppDbContext db) => _db = db;

        public IActionResult Index() => View();

        [HttpGet]
        public async Task<JsonResult> Listar()
        {
            var lista = await (from e in _db.FUT_ESTADISTICAS
                               join p in _db.FUT_PARTIDOS on e.PartidoId equals p.PartidoId
                               join el in _db.FUT_EQUIPOS on p.EquipoLocalId equals el.EquipoId
                               join ev in _db.FUT_EQUIPOS on p.EquipoVisitanteId equals ev.EquipoId
                               join j in _db.FUT_JUGADORES on e.JugadorId equals j.JugadorId
                               select new
                               {
                                   estadisticaId = e.EstadisticaId,
                                   partido = el.Nombre + " vs " + ev.Nombre,
                                   jugador = j.Nombre + " " + j.Apellido,
                                   goles = e.Goles,
                                   asistencias = e.Asistencias,
                                   tarjetasAmarillas = e.TarjetasAmarillas,
                                   tarjetasRojas = e.TarjetasRojas
                               })
                              .ToListAsync();

            return Json(new { data = lista });
        }

        [HttpGet]
        public async Task<JsonResult> Obtener(int id)
        {
            var e = await _db.FUT_ESTADISTICAS.FindAsync(id);
            if (e == null) return Json(null);

            return Json(new
            {
                estadisticaId = e.EstadisticaId,
                jugadorId = e.JugadorId,
                partidoId = e.PartidoId,
                goles = e.Goles,
                asistencias = e.Asistencias,
                tarjetasAmarillas = e.TarjetasAmarillas,
                tarjetasRojas = e.TarjetasRojas
            });
        }

        [HttpGet]
        public async Task<JsonResult> GetJugadores()
        {
            var jugadores = await _db.FUT_JUGADORES
                                     .Select(j => new
                                     {
                                         j.JugadorId,
                                         nombre = j.Nombre + " " + j.Apellido
                                     })
                                     .ToListAsync();
            return Json(jugadores);
        }
        
        [HttpGet]
        public async Task<JsonResult> GetPartidosPorJugador(int jugadorId)
        {
            var jugador = await _db.FUT_JUGADORES
                                   .AsNoTracking()
                                   .FirstOrDefaultAsync(j => j.JugadorId == jugadorId);
            if (jugador == null)
                return Json(new object[0]);

            var lista = await (from p in _db.FUT_PARTIDOS
                               where p.EquipoLocalId == jugador.EquipoId
                                  || p.EquipoVisitanteId == jugador.EquipoId
                               join el in _db.FUT_EQUIPOS on p.EquipoLocalId equals el.EquipoId
                               join ev in _db.FUT_EQUIPOS on p.EquipoVisitanteId equals ev.EquipoId
                               select new
                               {
                                   p.PartidoId,
                                   descripcion = el.Nombre + " vs " + ev.Nombre
                               })
                              .ToListAsync();

            return Json(lista);
        }

        [HttpPost]
        public async Task<JsonResult> Guardar([FromBody] Estadistica model)
        {
            bool ok = true;
            try
            {
                if (model.EstadisticaId == 0)
                    _db.FUT_ESTADISTICAS.Add(model);
                else
                    _db.FUT_ESTADISTICAS.Update(model);

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
                var e = await _db.FUT_ESTADISTICAS.FindAsync(id);
                if (e != null)
                {
                    _db.FUT_ESTADISTICAS.Remove(e);
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
