using Microsoft.AspNetCore.Mvc;

namespace LigasFutbol.Controllers
{
    public class InformeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
