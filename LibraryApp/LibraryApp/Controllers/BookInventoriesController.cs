using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Models;
using LibraryApp.Models.ViewModel;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace LibraryApp.Controllers
{
    public class BookInventoriesController : Controller
    {
        private readonly S22024Group4ProjectContext _context;

        public BookInventoriesController(S22024Group4ProjectContext context)
        {
            _context = context;
        }

        // GET: BookInventories
        public async Task<IActionResult> Index()
        {
            return View(await _context.BookInventories.ToListAsync());
        }
        [Authorize]
        public IActionResult getInventory()
        {
            string sql = @"SELECT 
                            BookCopy.ISBN, 
                            BookCopy.Edition, 
                            BookCopy.IsBorrowed, 
                            Book.Title, 
                            Book.Publisher, 
                            Book.PublishYear, 
                            Book.Description, 
                            Author.FirstName, 
                            Author.LastName
                        FROM 
                            BookCopy
                        INNER JOIN 
                            Book ON BookCopy.BookId = Book.BookId
                        INNER JOIN 
                            BookAuthor ON Book.BookId = BookAuthor.BookId
                        INNER JOIN 
                            Author ON BookAuthor.AuthorId = Author.AuthorId;";
            var report = _context.BookInventory.FromSqlRaw(sql).ToList();
            
            return View(report);
        }

        // GET: BookInventories/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookInventory = await _context.BookInventories
                .FirstOrDefaultAsync(m => m.ISBN == id);
            if (bookInventory == null)
            {
                return NotFound();
            }

            return View(bookInventory);
        }

        // GET: BookInventories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BookInventories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ISBN,Edition,IsBorrowed,Title,Publisher,PublishYear,Description,FirstName,LastName")] BookInventory bookInventory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bookInventory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bookInventory);
        }

        // GET: BookInventories/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookInventory = await _context.BookInventories.FindAsync(id);
            if (bookInventory == null)
            {
                return NotFound();
            }
            return View(bookInventory);
        }

        // POST: BookInventories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ISBN,Edition,IsBorrowed,Title,Publisher,PublishYear,Description,FirstName,LastName")] BookInventory bookInventory)
        {
            if (id != bookInventory.ISBN)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bookInventory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookInventoryExists(bookInventory.ISBN))
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
            return View(bookInventory);
        }

        // GET: BookInventories/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookInventory = await _context.BookInventories
                .FirstOrDefaultAsync(m => m.ISBN == id);
            if (bookInventory == null)
            {
                return NotFound();
            }

            return View(bookInventory);
        }

        // POST: BookInventories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var bookInventory = await _context.BookInventories.FindAsync(id);
            if (bookInventory != null)
            {
                _context.BookInventories.Remove(bookInventory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookInventoryExists(string id)
        {
            return _context.BookInventories.Any(e => e.ISBN == id);
        }
    }
}
