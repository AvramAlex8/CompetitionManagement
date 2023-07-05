using CompetitionManagement.Data;
using CompetitionManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CompetitionManagement.Controllers
{
    public class TeamController : Controller
    {
        private readonly CompetitionManagementContext _competitionManagementContext;
        public TeamController(CompetitionManagementContext competitionManagementContext)
        {
            _competitionManagementContext = competitionManagementContext;
        }
        public IActionResult Index()
        {
            return View(_competitionManagementContext.Teams.ToList());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Team team)
        {
            _competitionManagementContext.Teams.Add(team);
            _competitionManagementContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
