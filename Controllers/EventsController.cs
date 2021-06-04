using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ModelTrial.Data;
using ModelTrial.Models;
using Newtonsoft.Json;

namespace ModelTrial.Controllers
{
    public class EventsController : Controller
    {
        private readonly ModelTrialContext _context;
        public string ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=ModelTrialContext-da245adf-27ed-4c81-8611-a6e1abffab99;Trusted_Connection=True;MultipleActiveResultSets=true";
        
        public EventsController(ModelTrialContext context)
        {
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            return View(await _context.Event.ToListAsync());
        }

        // GET: Events/Details/5
        [TempData]
        public string name { get; set; }
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // var e = from m in _context.Event where (m.Id == id) select m;
            var @event =  _context.Event
                .FirstOrDefault(m => m.Id == id); 
            var u = _context.User.FromSqlRaw($"select * from Users where Id in (select CreatedByUserId from Event where Id = {id})").ToList()[0];
            name = u.Username;

            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event @event)
        {
            if (ModelState.IsValid)
            {
                var id = (int)TempData.Peek("Id");
                User u = _context.User.FromSqlRaw($"select * from Users where Id = {id}").ToList()[0];

                Event e = new Event { 
                    Title = @event.Title, 
                    Description = @event.Description, 
                    Date = @event.Date, 
                    Location = @event.Location,
                    ImageUrl = @event.ImageUrl, 
                    CreatedByUser = u
                };

                _context.Add(e);
                u.CreatedEvents.Add(e);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Manage));
            }
            return View(@event);
        }
        internal void InsertIntoDb(Event @event)
        {
            string queryString = "INSERT INTO dbo.Event VALUES (@Title, @Date, @Location, @Description, @ImageUrl, @CreatedByUserId)";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.Add("@Title", System.Data.SqlDbType.NVarChar).Value = @event.Title;
                command.Parameters.Add("@Date", System.Data.SqlDbType.DateTime2).Value = @event.Date;
                command.Parameters.Add("@Location", System.Data.SqlDbType.NVarChar).Value = @event.Location;
                command.Parameters.Add("@Description", System.Data.SqlDbType.NVarChar).Value = @event.Description;
                command.Parameters.Add("@ImageUrl", System.Data.SqlDbType.NVarChar).Value = @event.ImageUrl;
                command.Parameters.Add("@CreatedByUserId", System.Data.SqlDbType.Int).Value = TempData.Peek("UserId");

                try
                {
                    connection.Open();

                    int rowsAffected = command.ExecuteNonQuery();

                    command.Dispose();
                    connection.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Date,Location,Description,ImageUrl")] Event @event)
        {
            if (id != @event.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Manage));
            }
            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Event.FindAsync(id);
            _context.Event.Remove(@event);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Manage));
        }
        public IActionResult Manage()
        {
            var id = (int)TempData.Peek("Id");
            var events = _context.Event.FromSqlRaw($"select * from Event where CreatedByUserId = {id}").ToList();

            return View(events);
        }
        [HttpPost]
        public IActionResult Register(int eventId)
        {
            if (TempData.Peek("Id") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var id = (int)TempData.Peek("Id");
            Ticket ticket = new Ticket
            {
                UserId = id,
                EventId = eventId
            };
            _context.Ticket.Add(ticket);
            _context.SaveChanges();
            return RedirectToAction("Tickets", "Home");
        }

        private bool EventExists(int id)
        {
            return _context.Event.Any(e => e.Id == id);
        }
    }
}
