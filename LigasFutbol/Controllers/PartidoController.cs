using Microsoft.AspNetCore.Mvc;

namespace LigasFutbol.Controllers
{
    public class PartidoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
