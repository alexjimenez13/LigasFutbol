using Microsoft.AspNetCore.Mvc;

namespace LigasFutbol.Controllers
{
    public class EquipoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
