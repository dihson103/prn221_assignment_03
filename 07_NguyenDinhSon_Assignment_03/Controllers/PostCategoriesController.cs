using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _07_NguyenDinhSon_Assignment_03.Models;

namespace _07_NguyenDinhSon_Assignment_03.Controllers
{
    public class PostCategoriesController : Controller
    {
        private readonly AppDbContext _context;

        public PostCategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: PostCategories
        public async Task<IActionResult> Index()
        {
              return _context.PostCategories != null ? 
                          View(await _context.PostCategories.ToListAsync()) :
                          Problem("Entity set 'AppDbContext.PostCategories'  is null.");
        }

        // GET: PostCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.PostCategories == null)
            {
                return NotFound();
            }

            var postCategories = await _context.PostCategories
                .FirstOrDefaultAsync(m => m.CategoryID == id);
            if (postCategories == null)
            {
                return NotFound();
            }

            return View(postCategories);
        }

        // GET: PostCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PostCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryID,CategoryName,Description")] PostCategories postCategories)
        {
            if (ModelState.IsValid)
            {
                _context.Add(postCategories);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(postCategories);
        }

        // GET: PostCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.PostCategories == null)
            {
                return NotFound();
            }

            var postCategories = await _context.PostCategories.FindAsync(id);
            if (postCategories == null)
            {
                return NotFound();
            }
            return View(postCategories);
        }

        // POST: PostCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryID,CategoryName,Description")] PostCategories postCategories)
        {
            if (id != postCategories.CategoryID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(postCategories);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostCategoriesExists(postCategories.CategoryID))
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
            return View(postCategories);
        }

        // GET: PostCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.PostCategories == null)
            {
                return NotFound();
            }

            var postCategories = await _context.PostCategories
                .FirstOrDefaultAsync(m => m.CategoryID == id);
            if (postCategories == null)
            {
                return NotFound();
            }

            return View(postCategories);
        }

        // POST: PostCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.PostCategories == null)
            {
                return Problem("Entity set 'AppDbContext.PostCategories'  is null.");
            }
            var postCategories = await _context.PostCategories.FindAsync(id);
            if (postCategories != null)
            {
                _context.PostCategories.Remove(postCategories);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostCategoriesExists(int id)
        {
          return (_context.PostCategories?.Any(e => e.CategoryID == id)).GetValueOrDefault();
        }
    }
}
