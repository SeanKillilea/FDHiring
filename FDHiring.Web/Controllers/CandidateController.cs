using FDHiring.Core.Models;
using FDHiring.Data.Repositories;
using FDHiring.Web.Helpers;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
        public async Task<IActionResult> GetCandidate(int id)
        {
            var candidate = await _candidates.GetByIdAsync(id);
            if (candidate == null) return NotFound();
            return Json(candidate);
        }

        [HttpPost]
        public IActionResult SetCandidate(int candidateId)
        {
            HttpContext.Session.SetCandidateId(candidateId);
            return Ok();
        }

        [HttpPost]
        public IActionResult SaveSearchState(string? name, int? positionId, bool current, bool active)
        {
            HttpContext.Session.SetSearchState(name, positionId, current, active);
            return Ok();
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

        [HttpGet]
        public async Task<IActionResult> SearchCandidates(string? name, int? positionId, bool? current, bool? active)
        {
            var candidates = await _candidates.SearchAsync(name, positionId, current, active);
            var total = await _candidates.GetCountAsync();
            var results = candidates.ToList();

            return Json(new { total, showing = results.Count, candidates = results });
        }

        public async Task<IActionResult> Search()
        {
            ViewData["Title"] = "Search";

            var users = await _users.GetActiveAsync();
            var userId = HttpContext.Session.GetUserId();

            // Auto-set first user if none in session
            if (userId == 0 && users.Any())
            {
                var firstUser = users.First();
                HttpContext.Session.SetUser(firstUser.Id, firstUser.FirstName, firstUser.LastName);
                userId = firstUser.Id;
            }

            var searchState = HttpContext.Session.GetSearchState();

            ViewBag.Positions = await _positions.GetAllAsync();
            ViewBag.Users = users;
            ViewBag.CurrentUserId = userId;
            ViewBag.CurrentCandidateId = HttpContext.Session.GetCandidateId();
            ViewBag.SearchName = searchState.name;
            ViewBag.SearchPositionId = searchState.positionId;
            ViewBag.SearchCurrent = searchState.current;
            ViewBag.SearchActive = searchState.active;

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
            ViewBag.CandidateId = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCandidate([FromBody] Candidate candidate)
        {
            candidate.LastUpdatedByUserId = HttpContext.Session.GetUserId();
            await _candidates.UpdateAsync(candidate);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCandidate(int id)
        {
            await _candidates.DeleteAsync(id);
            HttpContext.Session.SetCandidateId(0);
            return Ok();
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