using CompetitionManagement.Data;
using CompetitionManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CompetitionManagement.Controllers
{
    public class PlayerController : Controller
    {
        private readonly CompetitionManagementContext _competitionManagementContext;
        public PlayerController(CompetitionManagementContext competitionManagementContext)
        {
            _competitionManagementContext = competitionManagementContext;
        }
        public IActionResult Index(string sortOrder)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.AgeSortParm = sortOrder == "Age" ? "age_desc" : "Age";
            ViewBag.TeamNameSortParm = sortOrder == "Team" ? "team_desc" : "Team";

            var players = _competitionManagementContext.Players.AsQueryable();

            switch (sortOrder)
            {
                case "name_desc":
                    players = players.Include(p => p.Team).OrderByDescending(p => p.LastName);
                    break;
                case "Age":
                    players = players.Include(p => p.Team).OrderBy(p => p.Age);
                    break;
                case "age_desc":
                    players = players.Include(p => p.Team).OrderByDescending(p => p.Age);
                    break;
                case "Team":
                    players = players.Include(p => p.Team).OrderBy(p => p.Team.Name);
                    break;
                case "team_desc":
                    players = players.Include(p => p.Team).OrderByDescending(p => p.Team.Name);
                    break;
                default:
                    players = players.Include(p => p.Team).OrderBy(p => p.LastName);
                    break;
            }

            return View(players.ToList());

        }
        public IActionResult Create()
        {
            ViewBag.Teams = _competitionManagementContext.Teams.ToList();
            return View();
        }
        [HttpPost]
        public IActionResult Create(Player player)
        {
            _competitionManagementContext.Players.Add(player);
            _competitionManagementContext.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Edit(int id)
        {
            Player player = _competitionManagementContext.Players.Include(p => p.Team).FirstOrDefault(p => p.Id == id);
            ViewBag.Teams = _competitionManagementContext.Teams.ToList();
            return View(player);
        }
        [HttpPost]
        public IActionResult Edit(Player player)
        {
            Player editedPlayer = _competitionManagementContext.Players.Find(player.Id);
            editedPlayer.FirstName = player.FirstName;
            editedPlayer.LastName = player.LastName;
            editedPlayer.Age = player.Age;
            editedPlayer.TeamId = player.TeamId;
            editedPlayer.Team = player.Team;
            _competitionManagementContext.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            Player player = _competitionManagementContext.Players.Include(p => p.Team).FirstOrDefault(p => p.Id == id);
            return View(player);
        }
        [HttpPost]
        public IActionResult Delete(Player player)
        {
            _competitionManagementContext.Players.Remove(player);
            _competitionManagementContext.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Details(int id)
        {
            Player player = _competitionManagementContext.Players.Include(p => p.Team).FirstOrDefault(t => t.Id == id);
            return View(player);
        }
    }
}
