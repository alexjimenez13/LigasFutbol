using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LigasFutbol.Data;
using LigasFutbol.Models;

namespace LigasFutbol.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        public HomeController(AppDbContext db) => _db = db;

        // GET: /
        public IActionResult Index() => View();

        // GET: /Home/GetUltimosPartidos
        [HttpGet]
        public async Task<JsonResult> GetUltimosPartidos()
        {
            var ultimos = await (from r in _db.FUT_RESULTADOS
                                 join p in _db.FUT_PARTIDOS on r.PartidoId equals p.PartidoId
                                 join el in _db.FUT_EQUIPOS on p.EquipoLocalId equals el.EquipoId
                                 join ev in _db.FUT_EQUIPOS on p.EquipoVisitanteId equals ev.EquipoId
                                 orderby p.FechaHora descending
                                 select new
                                 {
                                     Fecha = p.FechaHora.ToString("yyyy-MM-dd HH:mm"),
                                     Local = el.Nombre,
                                     Visitante = ev.Nombre,
                                     GolesLocal = r.GolesLocal,
                                     GolesVisita = r.GolesVisita
                                 })
                                .Take(5)
                                .ToListAsync();
            return Json(ultimos);
        }

        // GET: /Home/GetTopGoleadores
        [HttpGet]
        public async Task<JsonResult> GetTopGoleadores()
        {
            var top = await _db.FUT_ESTADISTICAS
                               .Include(e => e.Jugador)
                               .GroupBy(e => new { e.JugadorId, e.Jugador.Nombre, e.Jugador.Apellido })
                               .Select(g => new
                               {
                                   Jugador = g.Key.Nombre + " " + g.Key.Apellido,
                                   Goles = g.Sum(x => x.Goles)
                               })
                               .OrderByDescending(x => x.Goles)
                               .Take(5)
                               .ToListAsync();
            return Json(top);
        }

        // GET: /Home/GetClasificacionesPorLiga
        //[HttpGet]
        //public async Task<JsonResult> GetClasificacionesPorLiga()
        //{
        //    // Esta vista ya contiene todos los campos necesarios
        //    var clasif = await _db.FUT_CLASIFICACIONES
        //        .Select(c => new
        //        {
        //            Liga = c.Liga,
        //            Equipo = c.Equipo,
        //            PJ = c.PartidosJugados,
        //            G = c.Ganados,
        //            E = c.Empatados,
        //            P = c.Perdidos,
        //            GF = c.GF,
        //            GC = c.GC,
        //            DG = c.DG,
        //            Puntos = c.Puntos
        //        })
        //        .ToListAsync();
        //    return Json(clasif);
        //}

        // GET: /Home/GetProximosPartidos
        [HttpGet]
        public async Task<JsonResult> GetProximosPartidos()
        {
            var proximos = await (from p in _db.FUT_PARTIDOS
                                  join el in _db.FUT_EQUIPOS on p.EquipoLocalId equals el.EquipoId
                                  join ev in _db.FUT_EQUIPOS on p.EquipoVisitanteId equals ev.EquipoId
                                  where p.FechaHora > DateTime.Now
                                  orderby p.FechaHora
                                  select new
                                  {
                                      Fecha = p.FechaHora.ToString("yyyy-MM-dd HH:mm"),
                                      Local = el.Nombre,
                                      Visitante = ev.Nombre
                                  })
                                  .Take(5)
                                  .ToListAsync();
            return Json(proximos);
        }
    }
}
