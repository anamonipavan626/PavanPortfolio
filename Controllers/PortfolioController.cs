using Microsoft.AspNetCore.Mvc;

namespace PavanPortfolio.Controllers
{
    public class PortfolioController : Controller
    {
        public IActionResult PortFolio()
        {
            return View();
        }
    }
}
