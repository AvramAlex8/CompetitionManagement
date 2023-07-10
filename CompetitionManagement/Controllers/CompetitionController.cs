using CompetitionManagement.Data;
using CompetitionManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System;

namespace CompetitionManagement.Controllers
{
    public class CompetitionController : Controller
    {
        private readonly CompetitionManagementContext _competitionManagementContext;
        public CompetitionController(CompetitionManagementContext competitionManagementContext)
        {
            _competitionManagementContext = competitionManagementContext;
        }
        public IActionResult Index(string sortOrder)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.StartDateSortParm = sortOrder == "StartDate" ? "startdate_desc" : "StartDate";
            ViewBag.EndDateSortParm = sortOrder == "EndDate" ? "enddate_desc" : "EndDate";
            ViewBag.LocationSortParm = sortOrder == "Location" ? "location_desc" : "Location";
            ViewBag.TypeSortParm = sortOrder == "Type" ? "type_desc" : "Type";

            var competitions = _competitionManagementContext.Competitions.AsQueryable();

            switch (sortOrder)
            {
                case "name_desc":
                    competitions = competitions.Include(c => c.Type).OrderBy(c => c.Name);
                    break;
                case "startdate_desc":
                    competitions = competitions.Include(c => c.Type).OrderByDescending(c => c.StartDate);
                    break;
                case "enddate_desc":
                    competitions = competitions.Include(c => c.Type).OrderByDescending(c => c.EndDate);
                    break;
                case "location_desc":
                    competitions = competitions.Include(c => c.Type).OrderByDescending(c => c.Location);
                    break;
                case "type_desc":
                    competitions = competitions.Include(c => c.Type).OrderByDescending(c => c.Type);
                    break;
                default:
                    competitions = competitions.Include(c => c.Type).OrderBy(c => c.Name);
                    break;
            }

            return View(competitions.ToList());
        }
        public IActionResult Create()
        {
            ViewBag.CompetitionTypes = _competitionManagementContext.CompetitionTypes.ToList();
            return View();
        }
        [HttpPost]
        public IActionResult Create(Competition competition, IFormFile file)
        {
            if (_competitionManagementContext.Competitions.Any(c => c.Name == competition.Name))
            {
                ModelState.AddModelError("Name", "There is already a competition with the same name.");
                return View(competition);
            }
            if (file != null)
            {
                byte[] byteArray;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    byteArray = memoryStream.ToArray();
                }
                competition.Logo = byteArray;
            }

            _competitionManagementContext.Competitions.Add(competition);
            _competitionManagementContext.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Details(int id)
        {
            Competition competition = _competitionManagementContext.Competitions.Include(c => c.Type).Include(c => c.Teams).FirstOrDefault(c => c.Id == id);
            return View(competition);
        }
        public IActionResult Delete(int id)
        {
            Competition competition = _competitionManagementContext.Competitions.Include(c => c.Type).FirstOrDefault(c => c.Id == id);
            return View(competition);
        }
        [HttpPost]
        public IActionResult Delete(Competition competition)
        {

            _competitionManagementContext.Competitions.Remove(competition);
            _competitionManagementContext.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult AddTeam(string sortOrder)
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

        [HttpPost]
        public IActionResult AddTeam(int competitionId, List<Team> selectedTeams)
        {
            Competition competition = _competitionManagementContext.Competitions.Find(competitionId);
            foreach (var team in selectedTeams)
            {
                competition.Teams.Add(team);
            }
            _competitionManagementContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
