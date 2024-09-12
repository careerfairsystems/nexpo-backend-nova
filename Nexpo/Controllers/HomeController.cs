using Microsoft.AspNetCore.Mvc;

namespace Nexpo.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
