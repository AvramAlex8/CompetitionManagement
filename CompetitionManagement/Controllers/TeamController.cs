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
        public IActionResult Edit(int id)
        {
            Team team = _competitionManagementContext.Teams.Find(id);
            return View(team);
        }
        [HttpPost]
        public IActionResult Edit(Team team)
        {
            Team editedTeam = _competitionManagementContext.Teams.Find(team.Id);
            editedTeam.Name = team.Name;
            editedTeam.AwardNumber = team.AwardNumber;
            editedTeam.Motto = team.Motto;
            editedTeam.CreatedOn = team.CreatedOn;
            _competitionManagementContext.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            Team team = _competitionManagementContext.Teams.Find(id);
            return View(team);
        }
        [HttpPost]
        public IActionResult Delete(Team team)
        {
            var players = _competitionManagementContext.Players.Where(p => p.TeamId == team.Id);
            _competitionManagementContext.Players.RemoveRange(players);

            _competitionManagementContext.Teams.Remove(team);
            _competitionManagementContext.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Details(int id)
        {
            Team team = _competitionManagementContext.Teams.Include(t => t.Players).FirstOrDefault(t => t.Id == id);
            return View(team);
        }
    }
}
