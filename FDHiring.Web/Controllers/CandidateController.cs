using Microsoft.AspNetCore.Mvc;
using FDHiring.Data.Repositories;
using FDHiring.Web.Helpers;

namespace FDHiring.Web.Controllers
{
    public class CandidateController : Controller
    {
        private readonly CandidateRepository _candidates;
        private readonly PositionRepository _positions;
        private readonly AgencyRepository _agencies;
        private readonly UserRepository _users;

        public CandidateController(
            CandidateRepository candidates,
            PositionRepository positions,
            AgencyRepository agencies,
            UserRepository users)
        {
            _candidates = candidates;
            _positions = positions;
            _agencies = agencies;
            _users = users;
        }

        [HttpPost]
        public async Task<IActionResult> SetUser(int userId)
        {
            var user = await _users.GetByIdAsync(userId);
            if (user != null)
            {
                HttpContext.Session.SetUser(user.Id, user.FirstName, user.LastName);
            }
            return Ok();
        }

        public async Task<IActionResult> Search()
        {
            ViewData["Title"] = "Search";
            ViewBag.Positions = await _positions.GetAllAsync();
            ViewBag.Users = await _users.GetActiveAsync();
            ViewBag.CurrentUserId = HttpContext.Session.GetUserId();
            return View();
        }

        public async Task<IActionResult> Add()
        {
            ViewData["Title"] = "Add Candidate";
            ViewBag.Positions = await _positions.GetAllAsync();
            ViewBag.Agencies = await _agencies.GetAllAsync();
            return View();
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewData["Title"] = "Edit Candidate";
            ViewBag.Positions = await _positions.GetAllAsync();
            ViewBag.Agencies = await _agencies.GetAllAsync();
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