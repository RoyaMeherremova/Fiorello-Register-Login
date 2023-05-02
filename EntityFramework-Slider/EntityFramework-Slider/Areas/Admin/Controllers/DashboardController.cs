using Microsoft.AspNetCore.Mvc;

namespace EntityFramework_Slider.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {
        //islediyimz areanin adin bildirmelik her controlllerde mutleq
        [Area("Admin")]
        //adminPanel-HomePage name is Dashboard
        public IActionResult Index()
        {
            return View();
        }
    }
}
