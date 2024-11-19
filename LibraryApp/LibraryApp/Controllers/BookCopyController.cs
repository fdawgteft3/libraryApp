using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Models;

namespace LibraryApp.Controllers
{
    public class BookCopyController : Controller
    {
        private readonly S22024Group4ProjectContext _context;

        public BookCopyController(S22024Group4ProjectContext context)
        {
            _context = context;
        }

        // GET: BookCopy
        public async Task<IActionResult> Index()
        {
            var s22024Group4ProjectContext = _context.BookCopies.Include(b => b.Book);
            return View(await s22024Group4ProjectContext.ToListAsync());
        }

        // GET: BookCopy/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookCopy = await _context.BookCopies
                .Include(b => b.Book)
                .FirstOrDefaultAsync(m => m.ISBN == id);
            if (bookCopy == null)
            {
                return NotFound();
            }

            return View(bookCopy);
        }

        // GET: BookCopy/Create
        public IActionResult Create()
        {
            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "BookId");
            return View();
        }

        // POST: BookCopy/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ISBN,BookId,IsBorrowed,Edition,IsActive")] BookCopy bookCopy)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bookCopy);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "BookId", bookCopy.BookId);
            return View(bookCopy);
        }

        // GET: BookCopy/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookCopy = await _context.BookCopies.FindAsync(id);
            if (bookCopy == null)
            {
                return NotFound();
            }
            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "BookId", bookCopy.BookId);
            return View(bookCopy);
        }

        // POST: BookCopy/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ISBN,BookId,IsBorrowed,Edition,IsActive")] BookCopy bookCopy)
        {
            if (id != bookCopy.ISBN)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bookCopy);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookCopyExists(bookCopy.ISBN))
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
            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "BookId", bookCopy.BookId);
            return View(bookCopy);
        }

        // GET: BookCopy/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookCopy = await _context.BookCopies
                .Include(b => b.Book)
                .FirstOrDefaultAsync(m => m.ISBN == id);
            if (bookCopy == null)
            {
                return NotFound();
            }

            return View(bookCopy);
        }

        // POST: BookCopy/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var bookCopy = await _context.BookCopies.FindAsync(id);
            if (bookCopy != null)
            {
                _context.BookCopies.Remove(bookCopy);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookCopyExists(string id)
        {
            return _context.BookCopies.Any(e => e.ISBN == id);
        }
        [HttpPost]
        public async Task<IActionResult> returnBook(Borrow_Record br)
        {
            string baseURL = "http://localhost:7066/api/ReturnBook";
            string queryParameter = $"?RecordNumber={br.RecordNumber}";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync($"{baseURL}{queryParameter}");
            HttpContent content = response.Content;
            string data = await content.ReadAsStringAsync();
            return RedirectToAction("Index");
        }
    }
}
