using Microsoft.AspNetCore.Mvc;

namespace LigasFutbol.Controllers
{
    public class EstadisticaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
