using Microsoft.AspNetCore.Mvc;

namespace LigasFutbol.Controllers
{
    public class LigaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
