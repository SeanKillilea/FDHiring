using Microsoft.AspNetCore.Mvc;

namespace FDHiring.Web.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Dashboard";
            ViewData["SidebarActive"] = "dashboard";
            return View();
        }
    }
}