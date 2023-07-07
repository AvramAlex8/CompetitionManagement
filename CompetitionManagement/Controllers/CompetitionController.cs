using CompetitionManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CompetitionManagement.Controllers
{
    public class CompetitionController : Controller
    {
        private readonly CompetitionManagementContext _competitionManagementContext;
        public CompetitionController(CompetitionManagementContext competitionManagementContext)
        {
            _competitionManagementContext = competitionManagementContext;
        }
        public IActionResult Index()
        {
            return View(_competitionManagementContext.Competitions.Include(c => c.Type).ToList());
        }
    }
}
