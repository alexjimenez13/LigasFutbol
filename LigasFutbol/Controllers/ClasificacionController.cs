using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LigasFutbol.Data;
using LigasFutbol.Models;

namespace LigasFutbol.Controllers
{
    public class ClasificacionController : Controller
    {
        private readonly AppDbContext _db;
        public ClasificacionController(AppDbContext db) => _db = db;

        public IActionResult Index() => View();

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
        public async Task<JsonResult> Listar(int ligaId)
        {
            try
            {
                var resultados = await (from r in _db.FUT_RESULTADOS
                                        join p in _db.FUT_PARTIDOS on r.PartidoId equals p.PartidoId
                                        where p.LigaId == ligaId
                                        select new
                                        {
                                            LocalId = p.EquipoLocalId,
                                            VisitanteId = p.EquipoVisitanteId,
                                            GolesLocal = r.GolesLocal,
                                            GolesVisita = r.GolesVisita
                                        })
                                       .ToListAsync();

                var equipos = await _db.FUT_EQUIPOS
                                       .Where(e => e.LigaId == ligaId && e.Estado)
                                       .Select(e => new { e.EquipoId, e.Nombre })
                                       .ToListAsync();

                var estadisticas = equipos.ToDictionary(
                    e => e.EquipoId,
                    e => new Clasificacion
                    {
                        EquipoId = e.EquipoId,
                        NombreEquipo = e.Nombre,
                        PartidosJugados = 0,
                        Ganados = 0,
                        Empatados = 0,
                        Perdidos = 0,
                        GolesAFavor = 0,
                        GolesEnContra = 0,
                        DiferenciaGoles = 0,
                        Puntos = 0
                    }
                );

                foreach (var r in resultados)
                {
                    var local = estadisticas[r.LocalId];
                    local.PartidosJugados++;
                    local.GolesAFavor += r.GolesLocal;
                    local.GolesEnContra += r.GolesVisita;
                    if (r.GolesLocal > r.GolesVisita) { local.Ganados++; local.Puntos += 3; }
                    else if (r.GolesLocal < r.GolesVisita) { local.Perdidos++; }
                    else { local.Empatados++; local.Puntos++; }

                    var visitante = estadisticas[r.VisitanteId];
                    visitante.PartidosJugados++;
                    visitante.GolesAFavor += r.GolesVisita;
                    visitante.GolesEnContra += r.GolesLocal;
                    if (r.GolesVisita > r.GolesLocal) { visitante.Ganados++; visitante.Puntos += 3; }
                    else if (r.GolesVisita < r.GolesLocal) { visitante.Perdidos++; }
                    else { visitante.Empatados++; visitante.Puntos++; }
                }

                var tabla = estadisticas.Values
                    .Select(s => {
                        s.DiferenciaGoles = s.GolesAFavor - s.GolesEnContra;
                        return s;
                    })
                    .OrderByDescending(s => s.Puntos)
                    .ThenByDescending(s => s.DiferenciaGoles)
                    .ThenByDescending(s => s.GolesAFavor)
                    .Select((s, idx) => new {
                        Posicion = idx + 1,
                        Equipo = s.NombreEquipo,
                        PartidosJugados = s.PartidosJugados,
                        G = s.Ganados,
                        E = s.Empatados,
                        P = s.Perdidos,
                        GF = s.GolesAFavor,
                        GC = s.GolesEnContra,
                        DG = s.DiferenciaGoles,
                        Puntos = s.Puntos
                    })
                    .ToList();

                return Json(new { data = tabla });
            }
            catch (System.Exception ex)
            {
                return Json(new { data = new object[0], error = ex.Message });
            }
        }
    }
}
