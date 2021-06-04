using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ModelTrial.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ModelTrial.Data;
using Microsoft.EntityFrameworkCore;


namespace ModelTrial.Controllers
{
    public class HomeController : Controller
    {
        private readonly ModelTrialContext _context;

        private readonly ILogger<HomeController> _logger;

        public string ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=ModelTrialContext-da245adf-27ed-4c81-8611-a6e1abffab99;Trusted_Connection=True;MultipleActiveResultSets=true";

        public HomeController(ILogger<HomeController> logger, ModelTrialContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.Event.FromSqlRaw("select * from Event"));
        }

        public IActionResult Search()
        {
            string query = Request.Query["q"];

            return View(_context.Event.FromSqlRaw($"select * from Event where Title like '%{query}%'"));
        }
        public IActionResult Tickets()
        {
            var id = (int)TempData.Peek("Id");
            return View(_context.Event.FromSqlRaw($"select * from Event where Id in (select EventId from Ticket where UserId = {id})").ToList());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
