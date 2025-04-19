using Microsoft.AspNetCore.Mvc;

namespace LigasFutbol.Controllers
{
    public class JugadorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
