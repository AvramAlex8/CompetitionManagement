using CompetitionManagement.Data;
using CompetitionManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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
        public IActionResult Index(string sortOrder)
        {
            ViewBag.NameSortParm = sortOrder == "name_asc" ? "name_desc" : "name_asc";
            ViewBag.AwardNumberSortParm = sortOrder == "awardnumber_asc" ? "awardnumber_desc" : "awardnumber_asc";
            ViewBag.CreatedOnSortParm = sortOrder == "createdon_asc" ? "createdon_desc" : "createdon_asc";

            var teams = _competitionManagementContext.Teams.AsQueryable();

            switch (sortOrder)
            {
                case "name_asc":
                    teams = teams.OrderBy(t => t.Name);
                    break;
                case "name_desc":
                    teams = teams.OrderByDescending(t => t.Name);
                    break;
                case "awardnumber_asc":
                    teams = teams.OrderBy(t => t.AwardNumber);
                    break;
                case "awardnumber_desc":
                    teams = teams.OrderByDescending(t => t.AwardNumber);
                    break;
                case "createdon_asc":
                    teams = teams.OrderBy(t => t.CreatedOn);
                    break;
                case "createdon_desc":
                    teams = teams.OrderByDescending(t => t.CreatedOn);
                    break;
                default:
                    teams = teams.OrderBy(t => t.Name);
                    break;
            }

            return View(teams.ToList());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Team team, IFormFile file)
        {
            if (_competitionManagementContext.Teams.Any(t => t.Name == team.Name))
            {
                ModelState.AddModelError("Name", "There is already a team with the same name.");
                return View(team);
            }
            team.Logo = ConvertFileToByte(file);

            _competitionManagementContext.Teams.Add(team);
            _competitionManagementContext.SaveChanges();
            return RedirectToAction("Index");
        }
        private static byte[] ConvertFileToByte(IFormFile file)
        {
            if (file != null)
            {
                byte[] byteArray;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    byteArray = memoryStream.ToArray();
                }
                return byteArray;
            }
            return null;
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
            if (_competitionManagementContext.Teams.Any(t => t.Name == team.Name) && editedTeam.Name != team.Name)
            {
                ModelState.AddModelError("Name", "There is already a team with the same name.");
                return View(team);
            }
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
