using Microsoft.AspNetCore.Mvc;
using FDHiring.Data.Repositories;
using FDHiring.Web.Helpers;

namespace FDHiring.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly UserRepository _users;

        public UserController(UserRepository users)
        {
            _users = users;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var users = await _users.GetActiveAsync();
            return Json(users.Select(u => new
            {
                u.Id,
                u.FirstName,
                u.LastName
            }));
        }

        [HttpPost]
        public async Task<IActionResult> SetUser(int id)
        {
            var user = await _users.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            HttpContext.Session.SetUser(user.Id, user.FirstName, user.LastName);

            return Json(new
            {
                user.Id,
                user.FirstName,
                user.LastName
            });
        }
    }
}