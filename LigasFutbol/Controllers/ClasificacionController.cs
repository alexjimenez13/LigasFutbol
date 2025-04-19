using Microsoft.AspNetCore.Mvc;

namespace LigasFutbol.Controllers
{
    public class ClasificacionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
