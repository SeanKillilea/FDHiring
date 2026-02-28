using Microsoft.AspNetCore.Mvc;

namespace FDHiring.Web.Controllers
{
    public class SettingsController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Settings";
            ViewData["SidebarActive"] = "settings";
            return View();
        }
    }
}