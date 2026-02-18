using Microsoft.AspNetCore.Mvc;

namespace FDHiring.Web.Controllers
{
    public class CandidateController : Controller
    {
        public IActionResult Search()
        {
            ViewData["Title"] = "Search";
            return View();
        }

        public IActionResult Add()
        {
            ViewData["Title"] = "Add Candidate";
            return View();
        }

        public IActionResult Edit(int id)
        {
            ViewData["Title"] = "Edit Candidate";
            return View();
        }

        public IActionResult Workflow(int? id)
        {
            ViewData["Title"] = "Candidate Workflow";
            return View();
        }

        public IActionResult Interview(int? id)
        {
            ViewData["Title"] = "Candidate Interview";
            return View();
        }

        public IActionResult Files(int? id)
        {
            ViewData["Title"] = "Candidate Files";
            return View();
        }

        public IActionResult Communicate(int? id)
        {
            ViewData["Title"] = "Communicate";
            return View();
        }
    }
}