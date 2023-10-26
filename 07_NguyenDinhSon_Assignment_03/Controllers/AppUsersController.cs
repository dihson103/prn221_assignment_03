using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _07_NguyenDinhSon_Assignment_03.Models;
using Microsoft.AspNetCore.SignalR;

namespace _07_NguyenDinhSon_Assignment_03.Controllers
{
    public class AppUsersController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<SignalrServer> _signalRHub;

        public AppUsersController(AppDbContext context, IHubContext<SignalrServer> hub)
        {
            _context = context;
            _signalRHub = hub;
        }

        // GET: AppUsers
        public async Task<IActionResult> Index()
        {
              return _context.AppUsers != null ? 
                          View(await _context.AppUsers.ToListAsync()) :
                          Problem("Entity set 'AppDbContext.AppUsers'  is null.");
        }

        public IActionResult GetUsers()
        {
            return Ok(_context.AppUsers.ToList());
        }

        // GET: AppUsers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.AppUsers == null)
            {
                return NotFound();
            }

            var appUsers = await _context.AppUsers
                .FirstOrDefaultAsync(m => m.UserID == id);
            if (appUsers == null)
            {
                return NotFound();
            }

            return View(appUsers);
        }

        // GET: AppUsers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AppUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserID,FullName,Address,Email,Password")] AppUsers appUsers)
        {
            if (ModelState.IsValid)
            {
                if (checkEmailValid(appUsers.Email))
                {
                    _context.Add(appUsers);
                    await _context.SaveChangesAsync();
                    _signalRHub.Clients.All.SendAsync("CreateNewUser");
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("error", "Email is already exist.");
            }
            return View(appUsers);
        }

        private bool checkEmailValid(string email)
        {
            AppUsers? user = _context.AppUsers.SingleOrDefault(m => m.Email == email);
            return user == null;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("UserID,FullName,Address,Email,Password")] AppUsers appUsers)
        {
            if (ModelState.IsValid)
            {
                if (checkEmailValid(appUsers.Email))
                {
                    _context.Add(appUsers);
                    await _context.SaveChangesAsync();
                    _signalRHub.Clients.All.SendAsync("CreateNewUser");
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("error", "Email is already exist.");
            }
            return View(appUsers);
        }

        // GET: AppUsers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.AppUsers == null)
            {
                return NotFound();
            }

            var appUsers = await _context.AppUsers.FindAsync(id);
            if (appUsers == null)
            {
                return NotFound();
            }
            return View(appUsers);
        }

        // POST: AppUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserID,FullName,Address,Email,Password")] AppUsers appUsers)
        {
            if (id != appUsers.UserID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appUsers);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppUsersExists(appUsers.UserID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(appUsers);
        }

        // GET: AppUsers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.AppUsers == null)
            {
                return NotFound();
            }

            var appUsers = await _context.AppUsers
                .FirstOrDefaultAsync(m => m.UserID == id);
            if (appUsers == null)
            {
                return NotFound();
            }

            return View(appUsers);
        }

        // POST: AppUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.AppUsers == null)
            {
                return Problem("Entity set 'AppDbContext.AppUsers'  is null.");
            }
            var appUsers = await _context.AppUsers.FindAsync(id);
            if (appUsers != null)
            {
                _context.AppUsers.Remove(appUsers);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppUsersExists(int id)
        {
          return (_context.AppUsers?.Any(e => e.UserID == id)).GetValueOrDefault();
        }
    }
}
