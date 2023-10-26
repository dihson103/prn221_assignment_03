using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _07_NguyenDinhSon_Assignment_03.Models;
using _07_NguyenDinhSon_Assignment_03.Utils;
using Microsoft.AspNetCore.SignalR;

namespace _07_NguyenDinhSon_Assignment_03.Controllers
{
    public class PostsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<SignalrServer> _signalRHub;

        public PostsController(AppDbContext context, IHubContext<SignalrServer> hub)
        {
            _context = context;
            _signalRHub = hub;
        }

        [HttpGet]
        public IActionResult SearchById(int id)
        {
            return Ok(_context.Posts.SingleOrDefault(p => p.PostID == id));
        }

        // GET: Posts
        public async Task<IActionResult> Index(int? pageIndex, DateTime fromDate, DateTime toDate, string? search, bool showAll)
        {
            if(_context.Posts != null)
            {
                

                int pageSize = 1;
                IQueryable<Posts> posts;

                if (showAll)
                {
                    posts = from s in _context.Posts
                            select s;
                }

                else if (string.IsNullOrEmpty(search))
                {
                    posts = from s in _context.Posts
                            where s.CreatedDate.CompareTo(fromDate) >= 0
                            && s.CreatedDate.CompareTo(toDate) <= 0
                            select s;
                }
                else
                {
                    posts = from s in _context.Posts
                            where s.CreatedDate.CompareTo(fromDate) >= 0
                            && s.CreatedDate.CompareTo(toDate) <= 0
                            && (s.PostID.ToString().Contains(search) || s.Title.Contains(search) || s.Content.Contains(search))
                            select s;
                }
                
                PaginatedList<Posts> results = await PaginatedList<Posts>
                    .CreateAsync(posts.AsNoTracking(), pageIndex ?? 1, pageSize);
                ViewData["pageIndex"] = pageIndex ?? 1;
                ViewData["Result"] = results;
                ViewData["fromDate"] = fromDate;
                ViewData["toDate"] = toDate;
                ViewData["searchValue"] = search;
                return View();
            }

            return Problem("Entity set 'AppDbContext.Posts'  is null.");
        }

        //public IActionResult GetPosts(int? pageIndex, DateTime fromDate, DateTime toDate)
        //{

        //}

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var posts = await _context.Posts
                .FirstOrDefaultAsync(m => m.PostID == id);
            if (posts == null)
            {
                return NotFound();
            }

            return View(posts);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostID,AuthorID,Title,Content,PublishStatus,CategoryID")] Posts posts)
        {
            posts.CreatedDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                if (!UserExist(posts.AuthorID))
                {
                    ModelState.AddModelError("error", "Author does not already exist.");
                    return View(posts);
                }
                if (!CategoryExist(posts.CategoryID))
                {
                    ModelState.AddModelError("error", "Category does not already exist.");
                    return View(posts);
                }
                _context.Add(posts);
                await _context.SaveChangesAsync();
                _signalRHub.Clients.All.SendAsync("CreateNewPost");
                return RedirectToAction(nameof(Index));
            }
            return View(posts);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var posts = await _context.Posts.FindAsync(id);
            if (posts == null)
            {
                return NotFound();
            }
            return View(posts);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostID,AuthorID,CreatedDate,UpdatedDate,Title,Content,PublishStatus,CategoryID")] Posts posts)
        {
            if (id != posts.PostID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!UserExist(posts.AuthorID))
                    {
                        ModelState.AddModelError("error", "Author does not already exist.");
                        return View(posts);
                    }
                    if (!CategoryExist(posts.CategoryID))
                    {
                        ModelState.AddModelError("error", "Category does not already exist.");
                        return View(posts);
                    }
                    posts.UpdatedDate = DateTime.Now;
                    _signalRHub.Clients.All.SendAsync("EditPost", posts.PostID);
                    _context.Update(posts);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostsExists(posts.PostID))
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
            return View(posts);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var posts = await _context.Posts
                .FirstOrDefaultAsync(m => m.PostID == id);
            if (posts == null)
            {
                return NotFound();
            }

            return View(posts);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Posts == null)
            {
                return Problem("Entity set 'AppDbContext.Posts'  is null.");
            }
            var posts = await _context.Posts.FindAsync(id);
            if (posts != null)
            {
                _context.Posts.Remove(posts);
            }
            
            await _context.SaveChangesAsync();
            _signalRHub.Clients.All.SendAsync("DeletePost");
            return RedirectToAction(nameof(Index));
        }

        private bool PostsExists(int id)
        {
          return (_context.Posts?.Any(e => e.PostID == id)).GetValueOrDefault();
        }

        private bool UserExist(int userId)
        {
            return (_context.AppUsers?.Any(user => user.UserID== userId)).GetValueOrDefault();
        }

        private bool CategoryExist(int categoryId)
        {
            return (_context.PostCategories?.Any(category => category.CategoryID == categoryId)).GetValueOrDefault();  
        }
    }
}
