﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Models;
using LibraryApp.Models.ViewModel;
using Newtonsoft.Json;

namespace LibraryApp.Controllers
{
    public class BookController : Controller
    {
        private readonly S22024Group4ProjectContext _context;

        public BookController(S22024Group4ProjectContext context)
        {
            _context = context;
        }

        // GET: Book
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.ToListAsync();
            var books = await _context.Books.ToListAsync();
            var bookAuthors = await _context.BookAuthors.Include(ba => ba.Author).ToListAsync();

            var categoryData = categories.Select(c => new CategoryBookAuthors
            {
                Category = c,
                Books = books.Where(b => b.CategoryId == c.CategoryId).Select(b => new BookAuthors
                {
                    Book = b,
                    Authors = bookAuthors.Where(ba => ba.BookId == b.BookId).Select(ba => ba.Author).ToList()
                }).ToList()
            }).ToList();

            return View(categoryData);
        }

        public string IndexAjax(string searchString)
        {

            string sql = "select * from Author where FirstName like @p0";
            string wrapString = "%" + searchString + "%";
            List<Author> authors = _context.Authors.FromSqlRaw(sql, wrapString).ToList();
            string json = JsonConvert.SerializeObject(authors);
            return json;
        }


        // GET: Book/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Book/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: Book/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookId,CategoryId,Title,Publisher,PublishYear,Description")] Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", book.CategoryId);
            return View(book);
        }

        // GET: Book/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var categories = await _context.Categories.ToListAsync();
            
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            //Get authors list
            var bookAuthors = await _context.BookAuthors.Where(ba => ba.BookId == id).Select(ba => ba.Author).ToListAsync();

            var bookAuthorModel = new BookAuthors
            {
                Book = book,
                Authors = bookAuthors
            };

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", book.CategoryId);
            
            return View(bookAuthorModel);
        }

        // POST: Book/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookId,CategoryId,Title,Publisher,PublishYear,Description")] Book book)
        {
            if (id != book.BookId)
            {
                return NotFound();
                
            }
            

            

            if (ModelState.IsValid)
            {
                //Get authors list
                var bookAuthors = await _context.BookAuthors.Where(ba => ba.BookId == id).ToListAsync();
                //Print all form keys for testing
                foreach (var key in HttpContext.Request.Form.Keys)
                {
                    Console.WriteLine($"Form Key: {key}, Value: {HttpContext.Request.Form[key]}");
                }
                    try
                {
                    _context.Update(book);
                    foreach (var key in HttpContext.Request.Form.Keys)
                    {
                        Console.WriteLine($"Form Key: {key}, Value: {HttpContext.Request.Form[key]}");
                    }
                    //Find and delete author list
                    var previousAuthors = _context.BookAuthors.Where(ba => ba.BookId == id);
                    _context.BookAuthors.RemoveRange(previousAuthors);

                    var rawSelectedAuthors = HttpContext.Request.Form["authorList"];
                    var selectedAuthors = rawSelectedAuthors.Where(a => !string.IsNullOrEmpty(a)).ToList();
                    foreach (var authorId in selectedAuthors)
                    {
                        Console.WriteLine($"AuthorId: {authorId}");
                        int authorIdInt = int.Parse(authorId);

                        _context.BookAuthors.Add(new BookAuthor
                        {
                            BookId = id,
                            AuthorId = authorIdInt
                        });
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.BookId))
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
            else
            {
                var validationErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                foreach (var error in validationErrors)
                {
                    Console.WriteLine($"Validation error: {error}");
                }
                }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", book.CategoryId);
            return View(book);
        }

        // GET: Book/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Book/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.BookId == id);
        }
    }
}