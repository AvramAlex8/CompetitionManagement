using CompetitionManagement.Data;
using CompetitionManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;

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
            competition.Logo = ConvertFileToByte(file);

            _competitionManagementContext.Competitions.Add(competition);
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

        public IActionResult Details(int id, string errorMessage)
        {
            ViewBag.ErrorMessage = errorMessage;
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
        public IActionResult AddTeam(int id, string sortOrder)
        {
            ViewBag.NameSortParm = sortOrder == "name_asc" ? "name_desc" : "name_asc";
            ViewBag.AwardNumberSortParm = sortOrder == "awardnumber_asc" ? "awardnumber_desc" : "awardnumber_asc";
            ViewBag.CreatedOnSortParm = sortOrder == "createdon_asc" ? "createdon_desc" : "createdon_asc";

            var teams = _competitionManagementContext.Teams.ToList();
            Competition competition = _competitionManagementContext.Competitions.Include(c => c.Teams).FirstOrDefault(c => id == c.Id);
            foreach (var team in competition.Teams)
            {
                teams.Remove(team);
            }
            var goodTeams = teams.AsQueryable();

            switch (sortOrder)
            {
                case "name_asc":
                    goodTeams = goodTeams.OrderBy(t => t.Name);
                    break;
                case "name_desc":
                    goodTeams = goodTeams.OrderByDescending(t => t.Name);
                    break;
                case "awardnumber_asc":
                    goodTeams = goodTeams.OrderBy(t => t.AwardNumber);
                    break;
                case "awardnumber_desc":
                    goodTeams = goodTeams.OrderByDescending(t => t.AwardNumber);
                    break;
                case "createdon_asc":
                    goodTeams = goodTeams.OrderBy(t => t.CreatedOn);
                    break;
                case "createdon_desc":
                    goodTeams = goodTeams.OrderByDescending(t => t.CreatedOn);
                    break;
                default:
                    goodTeams = goodTeams.OrderBy(t => t.Name);
                    break;
            }

            return View(goodTeams.ToList());

        }

        [HttpPost]
        public IActionResult AddTeam(int id, List<int> selectedTeams)
        {
            Competition competition = _competitionManagementContext.Competitions.Find(id);
            foreach (var teamId in selectedTeams)
            {
                Team team = _competitionManagementContext.Teams.Find(teamId);
                competition.Teams.Add(team);
            }
            _competitionManagementContext.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Start(int id)
        {
            Competition competition = _competitionManagementContext.Competitions.Include(c => c.Teams).FirstOrDefault(c => id == c.Id);
            if (competition.Teams.Count < 2)
            {
                string errorMessage = "You need at least 2 teams to start the competition.";
                return RedirectToAction("Details", new { id, errorMessage });
            }
            GenerateMatchesRoundRobin(competition);
            // TODO - generate matches based on competition type
            competition.Started = true;
            _competitionManagementContext.SaveChanges();
            return RedirectToAction("Standings", new { id });
        }

        private void GenerateMatchesRoundRobin(Competition competition)
        {
            int teamCount = competition.Teams.Count;

            for (int i = 0; i < teamCount - 1; i++)
            {
                for (int j = i + 1; j < teamCount; j++)
                {
                    Team team1 = competition.Teams.ElementAt(i);
                    Team team2 = competition.Teams.ElementAt(j);
                    Game game = new Game
                    {
                        Team1Id = team1.Id,
                        Team2Id = team2.Id,
                        CompetitionId = competition.Id,
                        Date = DateTime.Today,
                        Stadion = competition.Location,
                        Team1Name = team1.Name,
                        Team2Name = team2.Name,
                        Team1 = team1,
                        Team2 = team2,
                        // TODO - add a home stadion for team1
                        // TODO - make a team to play once per week
                    };
                    _competitionManagementContext.Games.Add(game);
                }
            }
            _competitionManagementContext.SaveChanges();
        }

        public IActionResult Standings(int id)
        {
            Competition competition = _competitionManagementContext.Competitions.Include(c => c.Teams).FirstOrDefault(c => id == c.Id);
            List<Team> teams = new List<Team>();
            foreach (var team in competition.Teams)
            {
                teams.Add(team);
            }
            List<Game> games = _competitionManagementContext.Games.Where(g => g.CompetitionId == competition.Id).ToList();

            Dictionary<Team, TeamCompetitionDetails> teamDetails = new Dictionary<Team, TeamCompetitionDetails>();

            foreach (Team team in teams)
            {
                int points = 0;
                int matches = 0;
                int wins = 0;
                int draws = 0;
                int loses = 0;
                int goalsScored = 0;
                int goalsConceded = 0;
                foreach (Game game in games)
                {
                    if (game.Team1Goals != -1)
                    {
                        if (game.Team1Id == team.Id)
                        {
                            matches++;
                            if (game.Team1Goals > game.Team2Goals)
                            {
                                points += 3;
                                wins++;
                            }
                            else if (game.Team1Goals == game.Team2Goals)
                            {
                                points += 1;
                                draws++;
                            }
                            else
                            {
                                loses++;
                            }
                            goalsScored += game.Team1Goals;
                            goalsConceded += game.Team2Goals;
                        }
                        else
                        {
                            if (game.Team2Id == team.Id)
                            {
                                matches++;
                                if (game.Team1Goals < game.Team2Goals)
                                {
                                    points += 3;
                                    wins++;
                                }
                                else if (game.Team1Goals == game.Team2Goals)
                                {
                                    points += 1;
                                    draws++;
                                }
                                else
                                {
                                    loses++;
                                }
                                goalsScored += game.Team2Goals;
                                goalsConceded += game.Team1Goals;
                            }
                        }
                    }
                }
                teamDetails[team] = new TeamCompetitionDetails()
                {
                    MatchesPlayed = matches,
                    Wins = wins,
                    Draws = draws,
                    Loses = loses,
                    GoalsScored = goalsScored,
                    GoalsConceded = goalsConceded,
                    Points = points
                };
                ViewBag.TeamDetails = teamDetails;
            }
            teams = teams.OrderByDescending(t => teamDetails[t].Points)
                .ThenBy(t => teamDetails[t].GoalsScored)
                .ThenBy(t => teamDetails[t].GoalsScored - teamDetails[t].GoalsConceded).ToList();

            ViewBag.CompetitionId = id;
            return View(teams);
        }

        public IActionResult Fixtures(int id)
        {
            Competition competition = _competitionManagementContext.Competitions.Include(c => c.Games).Include(c => c.Teams).FirstOrDefault(c => id == c.Id);
            List<Game> games = GetGamesFromCompetition(competition);
            games = games.Where(g => g.Team1Goals == -1).ToList();
            return View(games);
        }

        private List<Game> GetGamesFromCompetition(Competition competition)
        {
            return _competitionManagementContext.Games.Where(g => g.CompetitionId == competition.Id).ToList();
        }

        public IActionResult AddScore(int id, string homeTeamName, string awayTeamName)
        {
            ViewBag.HomeTeamName = homeTeamName;
            ViewBag.AwayTeamName = awayTeamName;
            Game game = _competitionManagementContext.Games.Include(g => g.Team1).Include(g => g.Team2).FirstOrDefault(g => g.Id == id);
            return View(game);
        }
        [HttpPost]
        public IActionResult AddScore(Game game)
        {
            Game editedGame = _competitionManagementContext.Games.Find(game.Id);
            editedGame.Team1Goals = game.Team1Goals;
            editedGame.Team2Goals = game.Team2Goals;
            _competitionManagementContext.SaveChanges();
            return RedirectToAction("Standings", new { id = editedGame.CompetitionId });
        }
        public IActionResult Results(int id)
        {
            Competition competition = _competitionManagementContext.Competitions.Include(c => c.Games).Include(c => c.Teams).FirstOrDefault(c => c.Id == id);
            List<Game> games = GetGamesFromCompetition(competition);
            games = games.Where(g => g.Team1Goals != -1).ToList();
            return View(games);
        }
    }
}
