using Microsoft.AspNetCore.Mvc;

namespace LigasFutbol.Controllers
{
    public class EntrenamientoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
